#include <iostream>
#include "opencv2/core/core.hpp"
#include "opencv2/highgui/highgui.hpp"
#include "opencv2/imgproc/imgproc.hpp"
#include <memory>
#include <chrono>
#include "cv.h"

#define SHOW_DEBUG 1

std::string GetColourName(TriColour colour)
{
	if (colour == TriColour::Red)
	{
		return "Red";
	}
	else if (colour == TriColour::Green)
	{
		return "Green";
	}
	else if (colour == TriColour::Blue)
	{
		return "Blue";
	}
	else if (colour == TriColour::Yellow)
	{
		return "Yellow";
	}

	return "INVALID";
}

const double CLOSE_CENTER_DIST2 = 10;
double CANNY_THRESH_MIN = 100;
double CANNY_THRESH_MAX = 200;
double EPISILON_PERCENTAGE = 0.1;
int POLYGON_AREA_MIN_THRESH = 700;
int FRAME_CONSISTENCY_HISTORY = 20;

// DEBUGGING
int minSlider = (int)CANNY_THRESH_MIN;
int maxSlider = (int)CANNY_THRESH_MAX;
int epsSlider = (int)(EPISILON_PERCENTAGE * 100);

void OnCannyMinTrackbar(int, void*)
{
	CANNY_THRESH_MIN = (double)minSlider;
}

void OnCannyMaxTrackbar(int, void*)
{
	CANNY_THRESH_MAX = (double)maxSlider;
}

void OnEpisilonTrackbar(int, void*)
{
	EPISILON_PERCENTAGE = ((double)epsSlider) / 100;
}
// END DEBUGGING

double EucDistance2(cv::Point& p1, cv::Point& p2)
{
	return (p1.x - p2.x)*(p1.x - p2.x) + (p1.y - p2.y)*(p1.y - p2.y);
}

bool HasCloseCenter(std::vector<PyraTriangle>& triangles, PyraTriangle& currentTri, double desiredDist)
{
	for (auto& candidate : triangles)
	{
		double dist = EucDistance2(candidate.center, currentTri.center);
		if (dist < desiredDist)
		{
			return true;
		}
	}

	return false;
}

cv::Mat GetColourCanny(cv::Mat& frame)
{
	cv::Mat bgr[3];
	cv::split(frame, bgr); // TODO: This is probably an unnecessary copy

	cv::Mat imcannyb, imcannyg, imcannyr;
	cv::Canny(bgr[0], imcannyb, CANNY_THRESH_MIN, CANNY_THRESH_MAX);
	cv::Canny(bgr[1], imcannyg, CANNY_THRESH_MIN, CANNY_THRESH_MAX);
	cv::Canny(bgr[2], imcannyr, CANNY_THRESH_MIN, CANNY_THRESH_MAX);

	cv::Mat imcanny; // TODO: This might be a copy
	cv::add(imcannyb, imcannyg, imcanny);
	cv::add(imcanny, imcannyr, imcanny);

	return imcanny;
}

std::vector<PyraTriangle> GetFaceTriangles(std::vector<std::vector<cv::Point>>& contours)
{
	std::vector<PyraTriangle> triangles;
	std::vector<double> areas;

	// Get valid triangles out of the contours
	for (auto& contour : contours)
	{
		PyraTriangle tri;

		tri.area = cv::contourArea(contour);
		if (tri.area < POLYGON_AREA_MIN_THRESH)
		{
			continue;
		}

		double epsilon = EPISILON_PERCENTAGE * cv::arcLength(contour, true);
		cv::approxPolyDP(contour, tri.polygon, epsilon, true);

		if (tri.polygon.size() != 3)
		{
			continue;
		}

		// TODO: Probably need to ensure we're not getting the overall center

		cv::Moments mom = cv::moments(contour); // TODO: this can probably be on the polygon instead
		tri.center = cv::Point((int)(mom.m10 / mom.m00), (int)(mom.m01 / mom.m00));

		// If we already have a center which is very close to this one, it's probably a duplicate
		// and we want to discard it
		if (HasCloseCenter(triangles, tri, CLOSE_CENTER_DIST2))
		{
			continue;
		}

		triangles.push_back(tri);
		areas.push_back(tri.area);
	}

	if (triangles.size() <= 9)
	{
		return triangles;
	}

	// Exclude triangles that are too far away from any other
	std::nth_element(areas.begin(), areas.begin() + areas.size() / 2, areas.end());
	double medianArea = areas[areas.size() / 2];
	double allowedDist = std::sqrt(medianArea) * 2;

	// TODO: Check this is actually removing things
	triangles.erase(
		std::remove_if(triangles.begin(), triangles.end(),
			[&](PyraTriangle& o) { return !HasCloseCenter(triangles, o, allowedDist); }),
		triangles.end());

	return triangles;
}

bool SortX(const PyraTriangle& lhs, const PyraTriangle& rhs)
{
	return lhs.center.x < rhs.center.x;
}

bool SortY(const PyraTriangle& lhs, const PyraTriangle& rhs)
{
	return lhs.center.y < rhs.center.y;
}

void ProcessUprightCenters(std::vector<PyraTriangle>& triangles, Face& face)
{
	// At this point we have 9 triangles.
	// Figure out what the triangles actually map to
	// We know for sure that the bottom left is min x, bottom right
	// is max x, and top is min y.

	// Sort by x first
	std::sort(std::begin(triangles), std::end(triangles), SortX);
	face.triangles[Face::BOTTOM_LEFT] = triangles[0];
	face.triangles[Face::BOTTOM_RIGHT] = triangles[8];

	// Sort by y now
	std::sort(std::begin(triangles), std::end(triangles), SortY);
	face.triangles[Face::TOP] = triangles[0];
	face.triangles[Face::MIDDLE_MIDDLE] = triangles[1];

	if (triangles[2].center.x < triangles[3].center.x)
	{
		face.triangles[Face::MIDDLE_LEFT] = triangles[2];
		face.triangles[Face::MIDDLE_RIGHT] = triangles[3];
	}
	else
	{
		face.triangles[Face::MIDDLE_LEFT] = triangles[3];
		face.triangles[Face::MIDDLE_RIGHT] = triangles[2];
	}

	std::vector<PyraTriangle> bottoms(5);
	bottoms[0] = triangles[4];
	bottoms[1] = triangles[5];
	bottoms[2] = triangles[6];
	bottoms[3] = triangles[7];
	bottoms[4] = triangles[8];
	std::sort(std::begin(bottoms), std::end(bottoms), SortX);

	face.triangles[Face::BOTTOM_MID_LEFT] = bottoms[1];
	face.triangles[Face::BOTTOM_MIDDLE] = bottoms[2];
	face.triangles[Face::BOTTOM_MID_RIGHT] = bottoms[3];
}

void ProcessUpsideDownCenters(std::vector<PyraTriangle>& triangles, Face& face)
{
	// At this point we have 9 triangles.
	// Figure out what the triangles actually map to
	// We know for sure that the bottom right is min x, bottom left
	// is max x, and top is max y.

	// Sort by x first
	std::sort(std::begin(triangles), std::end(triangles), SortX);
	face.triangles[Face::BOTTOM_RIGHT] = triangles[0];
	face.triangles[Face::BOTTOM_LEFT] = triangles[8];

	// Sort by y now
	std::sort(std::begin(triangles), std::end(triangles), SortY);
	face.triangles[Face::TOP] = triangles[8];
	face.triangles[Face::MIDDLE_MIDDLE] = triangles[7];

	if (triangles[5].center.x < triangles[6].center.x)
	{
		face.triangles[Face::MIDDLE_LEFT] = triangles[6];
		face.triangles[Face::MIDDLE_RIGHT] = triangles[5];
	}
	else
	{
		face.triangles[Face::MIDDLE_LEFT] = triangles[5];
		face.triangles[Face::MIDDLE_RIGHT] = triangles[6];
	}

	std::vector<PyraTriangle> bottoms(5);
	bottoms[0] = triangles[0];
	bottoms[1] = triangles[1];
	bottoms[2] = triangles[2];
	bottoms[3] = triangles[3];
	bottoms[4] = triangles[4];
	std::sort(std::begin(bottoms), std::end(bottoms), SortX);

	face.triangles[Face::BOTTOM_MID_LEFT] = bottoms[3];
	face.triangles[Face::BOTTOM_MIDDLE] = bottoms[2];
	face.triangles[Face::BOTTOM_MID_RIGHT] = bottoms[1];
}

TriColour GetColourFromHS(int hue, int sat)
{
	if (hue < 40 && sat > 200)
	{
		return TriColour::Red;
	}
	else if (hue < 70)
	{
		return TriColour::Yellow;
	}
	else if (hue < 80 && sat < 100)
	{
		return TriColour::Yellow;
	}
	else if (hue < 95)
	{
		return TriColour::Green;
	}
	else if (hue < 120)
	{
		return TriColour::Blue;
	}
	else if (hue > 120)
	{
		return TriColour::Red;
	}
	else
	{
		return TriColour::Invalid;
	}
}

void UpdateTriangleColours(cv::Mat& img, Face& face)
{
	cv::Mat imhsv;
	cv::cvtColor(img, imhsv, cv::COLOR_BGR2HSV);

	cv::Mat splithsv[3];
	cv::split(imhsv, splithsv);
#if SHOW_DEBUG
	cv::imshow("hue", splithsv[0]);
	cv::imshow("sat", splithsv[1]);
#endif
	int size = (int)(std::sqrt(face.GetMeanArea()) / 3);

	// Now to figure out what colours are in the centers
	// Sample the colour around the center in a square
	std::vector<int> hues(9);
	std::vector<int> sats(9);

	for (int i = 0; i < face.triangles.size(); i++)
	{
		auto& tri = face.triangles[i];
		auto yRange = cv::Range(tri.center.y - size, tri.center.y + size);
		auto xRange = cv::Range(tri.center.x - size, tri.center.x + size);
		cv::Mat huePixels = splithsv[0](yRange, xRange);
		cv::Mat satPixels = splithsv[1](yRange, xRange);
		hues[i] = (cv::mean(huePixels).val[0]);
		sats[i] = (cv::mean(satPixels).val[0]);

		face.colours[i] = GetColourFromHS(hues[i], sats[i]);
#if SHOW_DEBUG
		std::cout << GetColourName(face.colours[i]) << ": h = " << hues[i] << ", s = " << sats[i] << std::endl;
#endif
	}
}

void DrawTriangleInfo(cv::Mat& img, Face& face, int index, const std::string& name)
{
	PyraTriangle& tri = face.triangles[index];
	cv::circle(img, tri.center, 4, cv::Scalar(0, 255, 0), -1);
	cv::putText(img, std::to_string(tri.center.x) + "," + std::to_string(tri.center.y) + " " + name, cv::Point(tri.center.x - 20, tri.center.y + 20), cv::FONT_HERSHEY_SIMPLEX, 0.4, cv::Scalar(255, 255, 255), 1);
}

bool ProcessFrame(cv::Mat& frame, Face& face, bool useUpright)
{
	cv::Mat imcanny = GetColourCanny(frame);
#if SHOW_DEBUG
	cv::imshow("imcanny", imcanny);
#endif

	std::vector<std::vector<cv::Point>> contours;
	cv::findContours(imcanny, contours, cv::RETR_LIST, cv::CHAIN_APPROX_SIMPLE);

	std::vector<PyraTriangle> triangles = GetFaceTriangles(contours);

#if SHOW_DEBUG
	cv::Mat impolys = frame.clone();
	for (auto& triangle : triangles)
	{
		cv::Point* approxPoints = triangle.polygon.data();
		int n = triangle.polygon.size();
		cv::polylines(impolys, &approxPoints, &n, 1, true, cv::Scalar(255, 0, 0), 3);
	}
	cv::imshow("polys", impolys);
#endif

	// Only proceed if we have the right number of triangles
	if (triangles.size() != 9)
	{
		return false;
	}

	if (useUpright)
	{
		ProcessUprightCenters(triangles, face);
	}
	else
	{
		ProcessUpsideDownCenters(triangles, face);
	}
	
	UpdateTriangleColours(frame, face);

#if SHOW_DEBUG
	cv::Mat imcenters = frame.clone();
	for (auto& triangle : triangles)
	{
		cv::circle(imcenters, triangle.center, 4, cv::Scalar(255, 0, 0), -1);
	}

	DrawTriangleInfo(imcenters, face, Face::BOTTOM_LEFT, "BL");
	DrawTriangleInfo(imcenters, face, Face::BOTTOM_RIGHT, "BR");
	DrawTriangleInfo(imcenters, face, Face::TOP, "TOP");
	DrawTriangleInfo(imcenters, face, Face::MIDDLE_LEFT, "ML");
	DrawTriangleInfo(imcenters, face, Face::MIDDLE_MIDDLE, "MM");
	DrawTriangleInfo(imcenters, face, Face::MIDDLE_RIGHT, "MR");
	DrawTriangleInfo(imcenters, face, Face::BOTTOM_MID_LEFT, "BML");
	DrawTriangleInfo(imcenters, face, Face::BOTTOM_MIDDLE, "BM");
	DrawTriangleInfo(imcenters, face, Face::BOTTOM_MID_RIGHT, "BMR");

	cv::imshow("centers", imcenters);
#endif

	return true;
}

std::vector<std::vector<int>> colourCounts; // 9x5

bool ProcessFrameAndColours(cv::Mat& frame, Face& face, bool useUpright)
{
	if (ProcessFrame(frame, face, useUpright))
	{
		// We successfully got colour data from a frame, update counts
		for (int i = 0; i < 9; i++)
		{
			colourCounts[i][face.colours[i]]++;
		}

		return true;
	}

	return false;
}

void ResetColourCounts()
{
	colourCounts = std::vector<std::vector<int>>(9);
	for (int i = 0; i < 9; i++)
	{
		colourCounts[i] = std::vector<int>(5);

		for (int j = 0; j < 5; j++)
		{
			colourCounts[i][j] = 0;
		}
	}
}

std::vector<std::vector<int>>& GetColourCounts()
{
	return colourCounts;
}

std::vector<TriColour> GetWinningColours()
{
	std::vector<TriColour> result(9);
	for (int i = 0; i < 9; i++)
	{
		auto max = std::max_element(std::begin(colourCounts[i]), std::end(colourCounts[i]));
		TriColour colour = (TriColour)std::distance(std::begin(colourCounts[i]), max);
		result[i] = colour;
		//std::cout << GetColourName(colour) << " - " << colourCounts[i][colour] << std::endl;
	}

	return result;
}
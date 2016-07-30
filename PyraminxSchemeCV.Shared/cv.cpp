#include <iostream>
#include "opencv2/core/core.hpp"
#include "opencv2/highgui/highgui.hpp"
#include "opencv2/imgproc/imgproc.hpp"
#include <memory>
#include <chrono>

#define SHOW_DEBUG 1

enum TriColour {
	Red, Green, Blue, Yellow, Invalid
};

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

class PyraTriangle
{
public:
	double area;
	std::vector<cv::Point> polygon;
	cv::Point center;
};

class Face
{
public:
	static const int TOP = 0;
	static const int MIDDLE_LEFT = 1;
	static const int MIDDLE_MIDDLE = 2;
	static const int MIDDLE_RIGHT = 3;
	static const int BOTTOM_LEFT = 4;
	static const int BOTTOM_MID_LEFT = 5;
	static const int BOTTOM_MIDDLE = 6;
	static const int BOTTOM_MID_RIGHT = 7;
	static const int BOTTOM_RIGHT = 8;

	std::vector<PyraTriangle> triangles = std::vector<PyraTriangle>(9); // These triangles are ordered according to [top, middle_left, middle_middle, middle_right, bottom_left, bottom_mleft, bottom_middle, bottom_mright, bottom_right]
	std::vector<TriColour> colours = std::vector<TriColour>(9);

	double GetMeanArea()
	{
		double mean = 0;
		for (auto& tri : triangles)
		{
			mean += tri.area;
		}
		return mean / (double)triangles.size();
	}
	// TODO: Accessor functions
};

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

bool ProcessFrame(cv::Mat& frame, Face& face)
{
	cv::Mat flipped; // TODO: static alloc
	cv::flip(frame, flipped, 1);
	cv::Mat imsize; // TODO: static alloc
	cv::resize(flipped, imsize, cv::Size(0, 0), 0.8, 0.8);

	cv::Mat imcanny = GetColourCanny(imsize);
#if SHOW_DEBUG
	cv::imshow("imcanny", imcanny);
#endif

	std::vector<std::vector<cv::Point>> contours;
	cv::findContours(imcanny, contours, cv::RETR_LIST, cv::CHAIN_APPROX_SIMPLE);

	std::vector<PyraTriangle> triangles = GetFaceTriangles(contours);

#if SHOW_DEBUG
	cv::Mat impolys = imsize.clone();
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

	ProcessUprightCenters(triangles, face);
	UpdateTriangleColours(imsize, face);

#if SHOW_DEBUG
	cv::Mat imcenters = imsize.clone();
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
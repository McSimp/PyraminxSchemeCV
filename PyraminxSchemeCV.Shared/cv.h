#pragma once
#include <vector>
#include <opencv2/core/core.hpp>

enum TriColour {
	Red, Green, Blue, Yellow, Invalid
};

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

void ResetColourCounts();
bool ProcessFrameAndColours(cv::Mat& frame, Face& face, bool useUpright);
std::vector<std::vector<int>>& GetColourCounts();
std::string GetColourName(TriColour colour);
std::vector<TriColour> GetWinningColours();
cv::Mat& GetPolyMat();

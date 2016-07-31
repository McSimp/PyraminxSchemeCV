#include <iostream>
#include <opencv2/imgproc/imgproc.hpp>
#include <opencv2/highgui/highgui.hpp>
#include "cv.h"
#define SHOW_DEBUG 1

cv::VideoCapture* cap = nullptr;

extern "C" {
	__declspec(dllexport) void StartCapture()
	{
		cap = new cv::VideoCapture(0);
		ResetColourCounts();
	}

	__declspec(dllexport) void EndCapture()
	{
		if (cap != nullptr)
		{
			cv::destroyAllWindows();
			delete cap;
			cap = nullptr;
		}
	}

	__declspec(dllexport) void extResetColourCounts()
	{
		ResetColourCounts();
	}

	__declspec(dllexport) bool GetColours(bool mirrored, int expectedOrientation, int* colours)
	{
		cv::Mat frame;
		cap->read(frame);
		cv::imshow("camera", frame);

		cv::Mat flipped;
		cv::Mat& imToSize = frame;

		if (mirrored)
		{
			cv::flip(frame, flipped, 1);
			imToSize = flipped;
		}

		cv::Mat imsize;
		cv::resize(imToSize, imsize, cv::Size(0, 0), 0.8, 0.8);

		bool useUpright = (expectedOrientation == 0) ? true : false;

		Face face;
		bool result = ProcessFrameAndColours(frame, face, useUpright);

		std::vector<TriColour> winningColours = GetWinningColours();
		for (int i = 0; i < winningColours.size(); i++)
		{
			colours[i] = (int)winningColours[i];
		}

		return result;
	}
}

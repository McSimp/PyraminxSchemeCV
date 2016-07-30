#include <iostream>

extern "C" {
	bool GetColours(void* texture, int expectedOrientation, int* colours)
	{
		return true;
	}
}

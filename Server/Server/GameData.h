#pragma once
using namespace std;

struct Vector3
{
	float x;
	float y;
	float z;

	inline Vector3(void) {
		x = 0.0f;
		y = 0.0f;
		z = 0.0f;
	}
	inline Vector3(const float _x, const float _y, const float _z)
	{
		x = _x; y = _y; z = _z;
	}

	inline Vector3 operator + (const Vector3& A) const
	{
		return Vector3(x + A.x, y + A.y, z + A.z);
	}

	inline Vector3 operator + (const float A) const
	{
		return Vector3(x + A, y + A, z + A);
	}

	inline Vector3 operator - (const Vector3& A) const
	{
		return Vector3(x - A.x, y - A.y, z - A.z);
	}

	inline Vector3 operator - (const float A) const
	{
		return Vector3(x - A, y - A, z - A);
	}

	inline float Dot(const Vector3& A) const
	{
		return A.x * x + A.y * y + A.z * z;
	}
};
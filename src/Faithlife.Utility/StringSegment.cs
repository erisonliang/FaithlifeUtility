using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;

namespace Faithlife.Utility
{
	/// <summary>
	/// Encapsulates a length of characters from a string starting at a particular offset.
	/// </summary>
	[SuppressMessage("Microsoft.Design", "CA1036:OverrideMethodsOnComparableTypes", Justification = "String does not support comparison operators.")]
	[StructLayout(LayoutKind.Auto)]
	public struct StringSegment :
		IEnumerable<char>,
		IEquatable<StringSegment>,
		IComparable<StringSegment>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="StringSegment"/> class.
		/// </summary>
		/// <param name="source">The source string.</param>
		/// <remarks>Creates a segment that represents the entire source string.</remarks>
		public StringSegment(string source)
			: this(source, 0, source?.Length ?? 0)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="StringSegment"/> class.
		/// </summary>
		/// <param name="source">The source string.</param>
		/// <param name="offset">The offset of the segment.</param>
		/// <remarks>Creates a segment that starts at the specified offset and continues to the end
		/// of the source string.</remarks>
		/// <exception cref="ArgumentOutOfRangeException">The offset is out of range.</exception>
		public StringSegment(string source, int offset)
			: this(source, offset, (source?.Length ?? 0) - offset)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="StringSegment"/> class.
		/// </summary>
		/// <param name="source">The source string.</param>
		/// <param name="offset">The offset of the segment.</param>
		/// <param name="length">The length of the segment.</param>
		/// <exception cref="ArgumentOutOfRangeException">The offset or length are out of range.</exception>
		public StringSegment(string? source, int offset, int length)
		{
			int stringLength = source?.Length ?? 0;
			if (offset < 0 || offset > stringLength)
				throw new ArgumentOutOfRangeException(nameof(offset));
			if (length < 0 || offset + length > stringLength)
				throw new ArgumentOutOfRangeException(nameof(length));

			m_source = source ?? "";
			m_offset = offset;
			m_length = length;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="StringSegment"/> class.
		/// </summary>
		/// <param name="source">The source string.</param>
		/// <param name="capture">The <see cref="Capture" /> to represent.</param>
		public StringSegment(string source, Capture capture)
			: this(source, capture?.Index ?? 0, capture?.Length ?? 0)
		{
		}

		/// <summary>
		/// An empty segment of the empty string.
		/// </summary>
		public static readonly StringSegment Empty = new StringSegment(string.Empty);

		/// <summary>
		/// Gets the source string.
		/// </summary>
		/// <value>The source string of the segment.</value>
		public string Source => m_source;

		/// <summary>
		/// Gets the offset into the source string.
		/// </summary>
		/// <value>The offset into the source string of the segment.</value>
		public int Offset => m_offset;

		/// <summary>
		/// Gets the length of the segment.
		/// </summary>
		/// <value>The length of the segment.</value>
		public int Length => m_length;

		/// <summary>
		/// Gets the <see cref="Char"/> with the specified index.
		/// </summary>
		/// <value>The character at the specified index.</value>
		[System.Runtime.CompilerServices.IndexerName("Chars")]
		public char this[int index]
		{
			get
			{
				if (index < 0 || index >= m_length)
					throw new ArgumentOutOfRangeException(nameof(index));
				return m_source[m_offset + index];
			}
		}

		/// <summary>
		/// Returns everything that follows this segment in the source string.
		/// </summary>
		/// <returns>Everything that follows this segment in the source string.</returns>
		public StringSegment After() => new StringSegment(m_source, m_offset + m_length, m_source.Length - (m_offset + m_length));

		/// <summary>
		/// Appends the segment to the specified <see cref="StringBuilder" />.
		/// </summary>
		/// <param name="stringBuilder">The <see cref="StringBuilder" />.</param>
		public void AppendToStringBuilder(StringBuilder stringBuilder)
		{
			if (stringBuilder == null)
				throw new ArgumentNullException(nameof(stringBuilder));
			stringBuilder.Append(m_source, m_offset, m_length);
		}

		/// <summary>
		/// Returns everything that precedes this segment in the source string.
		/// </summary>
		/// <returns>Everything that precedes this segment in the source string.</returns>
		public StringSegment Before() => new StringSegment(m_source, 0, m_offset);

		/// <summary>
		/// Compares two string segments.
		/// </summary>
		/// <param name="segmentA">The first segment.</param>
		/// <param name="segmentB">The second segment.</param>
		/// <param name="comparison">The string comparison options.</param>
		/// <returns>Zero, a positive integer, or a negative integer, if the first segment is
		/// equal to, greater than, or less than the second segment, respectively.</returns>
		public static int Compare(StringSegment segmentA, StringSegment segmentB, StringComparison comparison)
		{
			int result = string.Compare(segmentA.Source, segmentA.m_offset, segmentB.Source, segmentB.m_offset, Math.Min(segmentA.m_length, segmentB.m_length), comparison);
			return CompareHelper(result, segmentA, segmentB);
		}

		/// <summary>
		/// Compares two string segments ordinally.
		/// </summary>
		/// <param name="segmentA">The first segment.</param>
		/// <param name="segmentB">The second segment.</param>
		/// <returns>Zero, a positive integer, or a negative integer, if the first segment is
		/// equal to, greater than, or less than the second segment, respectively.</returns>
		public static int CompareOrdinal(StringSegment segmentA, StringSegment segmentB)
		{
			int result = string.CompareOrdinal(segmentA.Source, segmentA.m_offset, segmentB.Source, segmentB.m_offset, Math.Min(segmentA.m_length, segmentB.m_length));
			return CompareHelper(result, segmentA, segmentB);
		}

		/// <summary>
		/// Compares the current object with another object of the same type.
		/// </summary>
		/// <param name="other">An object to compare with this object.</param>
		/// <returns>Zero, a positive integer, or a negative integer, if the first segment is
		/// equal to, greater than, or less than the second segment, respectively.</returns>
		public int CompareTo(StringSegment other)
			=> CultureInfo.CurrentCulture.CompareInfo.Compare(m_source, m_offset, m_length, other.m_source, other.m_offset, other.m_length, CompareOptions.None);

		/// <summary>
		/// Copies the characters of the string segment to an array.
		/// </summary>
		/// <param name="sourceIndex">The first character in the segment to copy.</param>
		/// <param name="destination">The destination array.</param>
		/// <param name="destinationIndex">The first index in the destination array.</param>
		/// <param name="count">The number of characters to copy.</param>
		public void CopyTo(int sourceIndex, char[] destination, int destinationIndex, int count)
			=> m_source.CopyTo(m_offset + sourceIndex, destination, destinationIndex, count);

		/// <summary>
		/// Indicates whether this instance and a specified object are equal.
		/// </summary>
		/// <param name="obj">Another object to compare to.</param>
		/// <returns>true if obj and this instance are the same type and represent the same value; otherwise, false.</returns>
		public override bool Equals(object obj) => obj is StringSegment && Equals((StringSegment) obj);

		/// <summary>
		/// Indicates whether the current object is equal to another object of the same type.
		/// </summary>
		/// <param name="other">An object to compare with this object.</param>
		/// <returns>true if the current object is equal to the other parameter; otherwise, false.</returns>
		public bool Equals(StringSegment other)
		{
			if (m_length != other.m_length)
				return false;

			if (object.ReferenceEquals(m_source, other.m_source) && m_offset == other.m_offset)
				return true;

			return CompareOrdinal(this, other) == 0;
		}

		/// <summary>
		/// Returns an enumerator that iterates through the characters of the string segment.
		/// </summary>
		/// <returns>A <see cref="T:System.Collections.Generic.IEnumerator`1"></see> that can be used to
		/// iterate through the characters of the string segment.</returns>
		public IEnumerator<char> GetEnumerator()
		{
			for (int index = 0; index < m_length; index++)
				yield return m_source[m_offset + index];
		}

		/// <summary>
		/// Returns the hash code for this instance.
		/// </summary>
		/// <returns>A 32-bit signed integer that is the hash code for this instance.</returns>
		/// <remarks>This hash code is identical to the hash code that results from
		/// calling <c>GetHashCode</c> on the result of <c>ToString</c>.</remarks>
		public override int GetHashCode() => ToString().GetHashCode();

		/// <summary>
		/// Returns the first index of the specified character in the string segment.
		/// </summary>
		/// <param name="value">The character to find.</param>
		/// <returns>The first index of the specified character in the string segment,
		/// or -1 if the character cannot be found.</returns>
		public int IndexOf(char value)
		{
			int index = m_source.IndexOf(value, m_offset, m_length);
			return index < 0 ? index : index - m_offset;
		}

		/// <summary>
		/// Returns the first index of the specified character in the string segment.
		/// </summary>
		/// <param name="value">The character to find.</param>
		/// <param name="startIndex">The search starting position.</param>
		/// <returns>The first index of the specified character in the string segment,
		/// or -1 if the character cannot be found.</returns>
		public int IndexOf(char value, int startIndex)
		{
			if (startIndex < 0 || startIndex >= m_length)
				throw new ArgumentOutOfRangeException(nameof(startIndex));

			int index = m_source.IndexOf(value, m_offset + startIndex, m_length - startIndex);
			return index < 0 ? index : index - m_offset;
		}

		/// <summary>
		/// Returns the first index of the specified string in the string segment.
		/// </summary>
		/// <param name="value">The string to find.</param>
		/// <param name="sc">The string comparison options.</param>
		/// <returns>The first index of the specified string in the string segment,
		/// or -1 if the string cannot be found.</returns>
		public int IndexOf(string value, StringComparison sc)
		{
			int index = m_source.IndexOf(value, m_offset, m_length, sc);
			return index < 0 ? index : index - m_offset;
		}

		/// <summary>
		/// Returns the first index of any of the specified characters in the string segment.
		/// </summary>
		/// <param name="anyOf">The characters to find.</param>
		/// <returns>The first index of any of the specified characters in the string segment,
		/// or -1 if none of the characters cannot be found.</returns>
		public int IndexOfAny(params char[] anyOf)
		{
			int index = m_source.IndexOfAny(anyOf, m_offset, m_length);
			return index < 0 ? index : index - m_offset;
		}

		/// <summary>
		/// Intersects this segment with another segment of the same string.
		/// </summary>
		/// <param name="segment">The segment to intersect.</param>
		/// <returns>A string segment that encapsulates the portion of the string
		/// contained by both string segments.</returns>
		/// <remarks>If the segments do not intersect, the segment will have a length
		/// of zero, but will be positioned at the offset of the end-most substring.</remarks>
		public StringSegment Intersect(StringSegment segment)
		{
			if (m_source != segment.m_source)
				throw new ArgumentException("The specified segment is from a different string.", nameof(segment));
			if (segment.m_offset >= m_offset + m_length)
				return Redirect(segment.m_offset, 0);
			if (segment.m_offset >= m_offset)
				return Redirect(segment.m_offset, Math.Min(segment.m_length, m_offset + m_length - segment.m_offset));
			if (m_offset >= segment.m_offset + segment.m_length)
				return Redirect(m_offset, 0);
			return Redirect(m_offset, Math.Min(m_length, segment.m_offset + segment.m_length - m_offset));
		}

		/// <summary>
		/// Returns the last index of the specified character in the string segment.
		/// </summary>
		/// <param name="value">The character to find.</param>
		/// <returns>The last index of the specified character in the string segment,
		/// or -1 if the character cannot be found.</returns>
		public int LastIndexOf(char value)
		{
			int index = m_source.LastIndexOf(value, m_offset + m_length - 1, m_length);
			return index < 0 ? index : index - m_offset;
		}

		/// <summary>
		/// Returns the last index of the specified string in the string segment.
		/// </summary>
		/// <param name="value">The string to find.</param>
		/// <param name="comparison">The string comparison options.</param>
		/// <returns>The last index of the specified string in the string segment,
		/// or -1 if the string cannot be found.</returns>
		public int LastIndexOf(string value, StringComparison comparison)
		{
			int index = m_source.LastIndexOf(value, m_offset + m_length - 1, m_length, comparison);
			return index < 0 ? index : index - m_offset;
		}

		/// <summary>
		/// Returns the last index of any of the specified characters in the string segment.
		/// </summary>
		/// <param name="anyOf">The characters to find.</param>
		/// <returns>The last index of any of the specified characters in the string segment,
		/// or -1 if none of the characters cannot be found.</returns>
		public int LastIndexOfAny(params char[] anyOf)
		{
			int index = m_source.LastIndexOfAny(anyOf, m_offset + m_length - 1, m_length);
			return index < 0 ? index : index - m_offset;
		}

		/// <summary>
		/// Matches the specified regex against the segment.
		/// </summary>
		/// <param name="regex">The regex to match.</param>
		/// <returns>The result of calling <c>regex.Match</c> on the segment.</returns>
		public Match Match(Regex regex)
		{
			if (regex == null)
				throw new ArgumentNullException(nameof(regex));
			return regex.Match(m_source, m_offset, m_length);
		}

		/// <summary>
		/// Returns a new <see cref="StringSegment"/> with the same owner string.
		/// </summary>
		/// <param name="offset">Offset of the new string segment.</param>
		/// <returns>A new segment with the same owner string.</returns>
		public StringSegment Redirect(int offset)
		{
			return new StringSegment(m_source, offset, m_source.Length - offset);
		}

		/// <summary>
		/// Returns a new <see cref="StringSegment"/> with the same owner string.
		/// </summary>
		/// <param name="offset">Offset of the new string segment.</param>
		/// <param name="length">Length of the new string segment.</param>
		/// <returns>A new segment with the same owner string.</returns>
		public StringSegment Redirect(int offset, int length)
		{
			return new StringSegment(m_source, offset, length);
		}

		/// <summary>
		/// Returns a new <see cref="StringSegment"/> with the same owner string.
		/// </summary>
		/// <param name="capture">The <see cref="Capture" /> to represent.</param>
		/// <returns>A new segment with the same owner string.</returns>
		public StringSegment Redirect(Capture capture)
		{
			if (capture == null)
				throw new ArgumentNullException(nameof(capture));
			return new StringSegment(m_source, capture.Index, capture.Length);
		}

		/// <summary>
		/// Splits the string segment by the specified regular expression.
		/// </summary>
		/// <param name="regex">The regular expression.</param>
		/// <returns>A collection of string segments corresponding to the split.
		/// Consult the <c>Split</c> method of the <c>Regex</c> class
		/// for more information.</returns>
		public IEnumerable<StringSegment> Split(Regex regex)
		{
			// find first match
			Match match = Match(regex);
			if (!match.Success)
			{
				// match not found, so yield this and we're done
				yield return this;
				yield break;
			}

			// ensure not right-to-left
			if (!regex.RightToLeft)
			{
				// loop through matches
				int resultOffset = m_offset;
				do
				{
					// yield segment before match
					yield return Redirect(resultOffset, match.Index - resultOffset);
					resultOffset = match.Index + match.Length;

					// yield captures
					GroupCollection groups = match.Groups;
					for (int nGroup = 1; nGroup < groups.Count; nGroup++)
					{
						Group group = groups[nGroup];
						if (group.Success)
							yield return Redirect(group);
					}

					// next match
					match = match.NextMatch();
				}
				while (match.Success);

				// yield segment after last match
				yield return Redirect(resultOffset, m_offset + m_length - resultOffset);
			}
			else
			{
				// loop through matches right to left
				int resultOffset = m_offset + m_length;
				do
				{
					// yield segment before match
					yield return Redirect(match.Index + match.Length, resultOffset - (match.Index + match.Length));
					resultOffset = match.Index;

					// yield captures
					GroupCollection groups = match.Groups;
					for (int index = 1; index < groups.Count; index++)
					{
						Group group = groups[index];
						if (group.Success)
							yield return Redirect(group);
					}

					// next match
					match = match.NextMatch();
				}
				while (match.Success);

				// yield segment after last match
				yield return Redirect(m_offset, resultOffset - m_offset);
			}
		}

		/// <summary>
		/// Returns a sub-segment of this segment.
		/// </summary>
		/// <param name="index">The start index into this segment.</param>
		/// <returns>A sub-segment of this segment</returns>
		public StringSegment Substring(int index) => Redirect(m_offset + index, m_length - index);

		/// <summary>
		/// Returns a sub-segment of this segment.
		/// </summary>
		/// <param name="index">The start index into this segment.</param>
		/// <param name="length">The length of the sub-segment.</param>
		/// <returns>A sub-segment of this segment</returns>
		public StringSegment Substring(int index, int length) => Redirect(m_offset + index, length);

		/// <summary>
		/// Trims the whitespace from the start and end of the string segment.
		/// </summary>
		/// <returns>A string segment with the whitespace trimmed.</returns>
		public StringSegment Trim() => TrimHelper(c_trimTypeBoth);

		/// <summary>
		/// Trims the specified characters from the start and end of the string segment.
		/// </summary>
		/// <param name="characters">The characters to trim.</param>
		/// <returns>A string segment with the whitespace trimmed.</returns>
		public StringSegment Trim(params char[] characters) => TrimHelper(characters, c_trimTypeBoth);

		/// <summary>
		/// Trims the specified characters from the end of the string segment.
		/// </summary>
		/// <returns>A string segment with the whitespace trimmed.</returns>
		public StringSegment TrimEnd() => TrimHelper(c_trimTypeEnd);

		/// <summary>
		/// Trims the whitespace from the end of the string segment.
		/// </summary>
		/// <param name="characters">The characters to trim.</param>
		/// <returns>A string segment with the whitespace trimmed.</returns>
		public StringSegment TrimEnd(params char[] characters) => TrimHelper(characters, c_trimTypeEnd);

		/// <summary>
		/// Trims the specified characters from the start of the string segment.
		/// </summary>
		/// <returns>A string segment with the whitespace trimmed.</returns>
		public StringSegment TrimStart() => TrimHelper(c_trimTypeStart);

		/// <summary>
		/// Trims the whitespace from the start of the string segment.
		/// </summary>
		/// <param name="characters">The characters to trim.</param>
		/// <returns>A string segment with the whitespace trimmed.</returns>
		public StringSegment TrimStart(params char[] characters) => TrimHelper(characters, c_trimTypeStart);

		/// <summary>
		/// Returns the array of characters represented by this string segment.
		/// </summary>
		/// <returns></returns>
		public char[] ToCharArray() => m_source.ToCharArray(m_offset, m_length);

		/// <summary>
		/// Returns the string segment as a string.
		/// </summary>
		/// <returns>The string segment as a string.</returns>
		public override string ToString() => m_source == null ? "" : m_source.Substring(m_offset, m_length);

		/// <summary>
		/// Returns the string segment as a <see cref="StringBuilder" />.
		/// </summary>
		/// <returns>The string segment as a <see cref="StringBuilder" />.</returns>
		public StringBuilder ToStringBuilder() => new StringBuilder(m_source, m_offset, m_length, 0);

		/// <summary>
		/// Returns the string segment as a <see cref="StringBuilder"/>.
		/// </summary>
		/// <param name="capacity">The capacity of the new <see cref="StringBuilder"/>.</param>
		/// <returns>The string segment as a <see cref="StringBuilder"/>.</returns>
		public StringBuilder ToStringBuilder(int capacity) => new StringBuilder(m_source, m_offset, m_length, capacity);

		/// <summary>
		/// Returns the union of this segment with another segment of the same string.
		/// </summary>
		/// <param name="segment">Another segment of the same string.</param>
		/// <returns>A string segment that spans both string segments.</returns>
		/// <remarks>If the segments do not intersect, the segment will also include any
		/// characters between the two segments.</remarks>
		public StringSegment Union(StringSegment segment)
		{
			if (m_source != segment.m_source)
				throw new ArgumentException("The specified segment is from a different string.", nameof(segment));
			int start = Math.Min(m_offset, segment.m_offset);
			int end = Math.Max(m_offset + m_length, segment.m_offset + segment.m_length);
			return Redirect(start, end - start);
		}

		/// <summary>
		/// Compares two string segments for equality.
		/// </summary>
		/// <param name="segmentA">The first string segment.</param>
		/// <param name="segmentB">The second string segment.</param>
		/// <returns><c>true</c> if the segments are equal; false otherwise.</returns>
		public static bool operator ==(StringSegment segmentA, StringSegment segmentB) => segmentA.Equals(segmentB);

		/// <summary>
		/// Compares two string segments for inequality.
		/// </summary>
		/// <param name="segmentA">The first string segment.</param>
		/// <param name="segmentB">The second string segment.</param>
		/// <returns><c>true</c> if the segments are not equal; false otherwise.</returns>
		public static bool operator !=(StringSegment segmentA, StringSegment segmentB) => !segmentA.Equals(segmentB);

		/// <summary>
		/// Returns an enumerator that iterates through the characters of the string segment.
		/// </summary>
		/// <returns>An <see cref="T:System.Collections.IEnumerator"></see> object that can be used to iterate
		/// through the characters of the string segment.</returns>
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => GetEnumerator();

		const int c_trimTypeStart = 0;
		const int c_trimTypeEnd = 1;
		const int c_trimTypeBoth = 2;

		private StringSegment TrimHelper(int trimType)
		{
			int start = m_offset;
			int end = m_offset + m_length;

			if (trimType != c_trimTypeEnd)
				while (start < end)
					if (char.IsWhiteSpace(m_source[start]))
						start++;
					else
						break;

			if (trimType != c_trimTypeStart)
				while (start < end)
					if (char.IsWhiteSpace(m_source[end - 1]))
						end--;
					else
						break;

			return Redirect(start, end - start);
		}

		private StringSegment TrimHelper(char[] trimChars, int trimType)
		{
			int start = m_offset;
			int end = m_offset + m_length;

			if (trimType != c_trimTypeEnd)
				while (start < end)
					if (Array.IndexOf(trimChars, m_source[start]) >= 0)
						start++;
					else
						break;

			if (trimType != c_trimTypeStart)
				while (start < end)
					if (Array.IndexOf(trimChars, m_source[end - 1]) >= 0)
						end--;
					else
						break;

			return Redirect(start, end - start);
		}

		// Compares the string length if the string segments otherwise compare equal
		private static int CompareHelper(int result, StringSegment segA, StringSegment segB) => result != 0 ? result : segA.m_length.CompareTo(segB.m_length);

		readonly string m_source;
		readonly int m_offset;
		readonly int m_length;
	}
}

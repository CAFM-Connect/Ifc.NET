using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ifc4
{

  public static class GlobalId
  {

      //  UniqueId = 60f91daf-3dd7-4283-a86d-24137b73f3da-0001fd0b;
      //  Dwf Guid = 60f91daf-3dd7-4283-a86d-24137b720ed1;
      //  Ifc Guid = 1W_HslFTT2WwXj91DxSWxH

      //  UniqueId = 60f91daf-3dd7-4283-a86d-24137b73f3da-0001fd1f;
      //  Dwf Guid = 60f91daf-3dd7-4283-a86d-24137b720ec5
      //  Ifc Guid = 1W_HslFTT2WwXj91DxSWx5

    private static readonly char[] base64Chars = new char[]
        { '0','1','2','3','4','5','6','7','8','9'
        , 'A','B','C','D','E','F','G','H','I','J'
        , 'K','L','M','N','O','P','Q','R','S','T'
        , 'U','V','W','X','Y','Z','a','b','c','d'
        , 'e','f','g','h','i','j','k','l','m','n'
        , 'o','p','q','r','s','t','u','v','w','x'
        , 'y','z','_','$' };

    /// <summary>
    /// Conversion of an integer into characters 
    /// with base 64 using the table base64Chars
    /// </summary>
    /// <param name="number">The number to convert</param>
    /// <param name="result">The result char array to write to</param>
    /// <param name="start">The position in the char array to start writing</param>
    /// <param name="len">The length to write</param>
    /// <returns></returns>
    static void cv_to_64( uint number, ref char[] result, int start, int len )
    {
      uint act;
      int iDigit, nDigits;

      Debug.Assert( len <= 4 );
      act = number;
      nDigits = len;

      for( iDigit = 0; iDigit < nDigits; iDigit++ )
      {
        result[start + len - iDigit - 1] = base64Chars[( int ) ( act % 64 )];
        act /= 64;
      }
      Debug.Assert( act == 0, "Logic failed, act was not null: " + act.ToString() );
      return;
    }

    /// <summary>
    /// The reverse function to calculate 
    /// the number from the characters
    /// </summary>
    /// <param name="str">The char array to convert from</param>
    /// <param name="start">Position in array to start read</param>
    /// <param name="len">The length to read</param>
    /// <returns>The calculated nuber</returns>
    static uint cv_from_64( char[] str, int start, int len )
    {
      int i, j, index;
      uint res = 0;
      Debug.Assert( len <= 4 );

      for( i = 0; i < len; i++ )
      {
        index = -1;
        for( j = 0; j < 64; j++ )
        {
          if( base64Chars[j] == str[start + i] )
          {
            index = j;
            break;
          }
        }
        Debug.Assert( index >= 0 );
        res = res * 64 + ( ( uint ) index );
      }
      return res;
    }

    /// <summary>
    /// Reconstruction of the GUID 
    /// from an IFC GUID string (base64)
    /// </summary>
    /// <param name="guid">The GUID string to convert. Must be 22 characters long</param>
    /// <returns>GUID correspondig to the string</returns>
    public static Guid ConvertFromIfcGUID(string ifcGlobalId)
    {
        Debug.Assert(ifcGlobalId.Length == 22, "Input string must not be longer that 22 chars");
      uint[] num = new uint[6];
      char[] str = ifcGlobalId.ToCharArray();
      int n = 2, pos = 0, i;
      for( i = 0; i < 6; i++ )
      {
        num[i] = cv_from_64( str, pos, n );
        pos += n; n = 4;
      }

      int a = ( int ) ( ( num[0] * 16777216 + num[1] ) );
      short b = ( short ) ( num[2] / 256 );
      short c = ( short ) ( ( num[2] % 256 ) * 256 + num[3] / 65536 );
      byte[] d = new byte[8];
      d[0] = Convert.ToByte( ( num[3] / 256 ) % 256 );
      d[1] = Convert.ToByte( num[3] % 256 );
      d[2] = Convert.ToByte( num[4] / 65536 );
      d[3] = Convert.ToByte( ( num[4] / 256 ) % 256 );
      d[4] = Convert.ToByte( num[4] % 256 );
      d[5] = Convert.ToByte( num[5] / 65536 );
      d[6] = Convert.ToByte( ( num[5] / 256 ) % 256 );
      d[7] = Convert.ToByte( num[5] % 256 );

      return new Guid( a, b, c, d );
    }

    /// <summary>
    /// Conversion of a GUID to a string 
    /// representing the GUID 
    /// </summary>
    /// <param name="guid">The GUID to convert</param>
    /// <returns>IFC (base64) encoded GUID string</returns>
    public static string ConvertToIfcGuid(Guid guid)
    {
      uint[] num = new uint[6];
      char[] str = new char[22];
      int i, n;
      byte[] b = guid.ToByteArray();

      // Creation of six 32 Bit integers from the components of the GUID structure
      num[0] = ( uint ) ( BitConverter.ToUInt32( b, 0 ) / 16777216 );
      num[1] = ( uint ) ( BitConverter.ToUInt32( b, 0 ) % 16777216 );
      num[2] = ( uint ) ( BitConverter.ToUInt16( b, 4 ) * 256 + BitConverter.ToUInt16( b, 6 ) / 256 );
      num[3] = ( uint ) ( ( BitConverter.ToUInt16( b, 6 ) % 256 ) * 65536 + b[8] * 256 + b[9] );
      num[4] = ( uint ) ( b[10] * 65536 + b[11] * 256 + b[12] );
      num[5] = ( uint ) ( b[13] * 65536 + b[14] * 256 + b[15] );

      // Conversion of the numbers into a system using a base of 64
      n = 2;
      int pos = 0;
      for( i = 0; i < 6; i++ )
      {
        cv_to_64( num[i], ref str, pos, n );
        pos += n; n = 4;
      }
      return new String( str );
    }

    //private static string s_ConversionTable_2X = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz_$";
    //public static string RevitConvertToIfcGuid(System.Guid guid)
    //{
    //    byte[] byteArray = guid.ToByteArray();
    //    ulong[] num = new ulong[6];
    //    num[0] = byteArray[3];
    //    num[1] = byteArray[2] * (ulong)65536 + byteArray[1] * (ulong)256 + byteArray[0];
    //    num[2] = byteArray[5] * (ulong)65536 + byteArray[4] * (ulong)256 + byteArray[7];
    //    num[3] = byteArray[6] * (ulong)65536 + byteArray[8] * (ulong)256 + byteArray[9];
    //    num[4] = byteArray[10] * (ulong)65536 + byteArray[11] * (ulong)256 + byteArray[12];
    //    num[5] = byteArray[13] * (ulong)65536 + byteArray[14] * (ulong)256 + byteArray[15];

    //    char[] buf = new char[22];
    //    int offset = 0;

    //    for (int ii = 0; ii < 6; ii++)
    //    {
    //        int len = (ii == 0) ? 2 : 4;
    //        for (int jj = 0; jj < len; jj++)
    //        {
    //            buf[offset + len - jj - 1] = s_ConversionTable_2X[(int)(num[ii] % 64)];
    //            num[ii] /= 64;
    //        }
    //        offset += len;
    //    }

    //    return new string(buf);
    //}

    //public static bool IsValidIfcGuid(string guid)
    //{
    //    if (guid == null)
    //        return false;

    //    if (guid.Length != 22)
    //        return false;

    //    foreach (char guidChar in guid)
    //    {
    //        if ((guidChar >= '0' && guidChar <= '9') ||
    //            (guidChar >= 'A' && guidChar <= 'Z') ||
    //            (guidChar >= 'a' && guidChar <= 'z') ||
    //            (guidChar == '_' || guidChar == '$'))
    //            continue;

    //        return false;
    //    }

    //    return true;
    //}

  }
}

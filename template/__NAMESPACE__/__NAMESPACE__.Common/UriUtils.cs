using System.Net;

namespace __NAMESPACE__.Common
{
    public static class UriUtils
    {
        public static string EncodeUri(string uriString)
            => WebUtility.UrlEncode(uriString);

        public static string DecodeUri(string encodedUriString)
            => WebUtility.UrlDecode(encodedUriString);

        public static string GetHostUri(string uriString)
        {
            try
            {
                return new Uri(uriString).GetLeftPart(UriPartial.Authority);
            }
            catch (Exception)
            {
                return null!;
            }
        }
    }
}

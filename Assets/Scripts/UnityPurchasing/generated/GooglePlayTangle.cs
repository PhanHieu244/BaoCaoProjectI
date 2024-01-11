// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("80HC4fPOxcrpRYtFNM7CwsLGw8AMrM+3UX9KQ0bCARp/FpPOJE/HVIdsjAIAWGTDJ2Lx5ZrbAMJPLISj0Fch0GHUjs09LkiA3ChcqJ5f+8nKtzzJQUyvpa3liGlTquI3N3D3rJCF/RXE4adQIFP9JrCO8E8BEOgSzEBgGE6u8Q9SnNXIy90iW+FoSyldSYF5j4W1lw9Q+ZI+AEAbNdILAkHCzMPzQcLJwUHCwsNr1J2DUrBel33Lwl+ZL7XFehcyqgNjIuHGTpUI1BZA7Z5Lyvg1G74iDl4C3ow6RXlLCpD0jZ/3ODDVWUHg609uc1PsCWkRC6KHqvS8o1g0e4Xw4tV/GOK2lWCwaLHsIAS5AO7Mqyyv7YZ5dfRaDwmvAISHrsHAwsPC");
        private static int[] order = new int[] { 0,9,13,8,6,10,8,12,9,11,10,12,13,13,14 };
        private static int key = 195;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}

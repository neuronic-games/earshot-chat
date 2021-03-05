namespace Whoo
{
    public static class GroupProps
    {
        #region Custom Property Keys

        public static string TableUserIsSittingAt(string userid) => $"SittingAtTable{userid}";
        public static string TableDisplay = "DisplayProperties";
        public static string TableCount   = "TableCount";
        public static string Stopwatch    = "Stopwatch";

        #endregion
    }
}
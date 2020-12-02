namespace Whoo
{
    public class GroupProps
    {
        #region Custom Property Keys

        public static string TableUserIsSittingAt(string userid) => $"SittingAtTable{userid}";
        public const  string TableDisplay = "DisplayProperties";
        public const  string TableCount   = "TableCount";
        public const  string Stopwatch    = "Stopwatch";

        #endregion
    }
}
using System;
using System.Collections.Generic;

[Serializable]
public class StrapiCommon
{
    //track changes
    public string id;
    public string _id;
    public string name;
    public string updatedAt;
    public string createdAt;
}

[Serializable]
public class ImageCommon
{
    public string hash;

    public string ext;

    public string mime;

    public float size;

    public int width;

    public int height;

    public string url;
}

[Serializable]
public class UserCommon
{
    public string _id;
    public string username;
    public string firstname;
    public string lastname;
    public string createdAt;
    public string updatedAt;
    public int    __v;
    public string id;
}

[Serializable]
public class RoomData : StrapiCommon
{
    #region Definitions

    [Serializable]
    public class TableData : StrapiCommon
    {
        public string[] zones;
        public int      x;
        public int      y;
        public string   updated_by;
        public string   created_by;

        public string zone;
    }

    [Serializable]
    public class ImageData : StrapiCommon
    {
        #region Definitions

        public class Format : ImageCommon
        {
            public string path;
        }

        public class Formats
        {
            public Format thumbnail;
            public Format medium;
            public Format small;
        }

        #endregion

        public string alternativeText;

        public string updated_by;

        public string created_by;

        public string caption;

        public Formats formats;

        public string provider;

        public string[] related;
    }

    #endregion

    public List<TableData> room_zones;

    public ImageData image;

    //room properties
    public int        capacity;
    public UserCommon created_by;
    public UserCommon updated_by;
}

[Serializable]
public class ZoneData : StrapiCommon
{
    public bool proximity;

    public UserCommon created_by;
    public UserCommon updated_by;

    public bool seated;

    public int x;
    public int y;
    public int width;
    public int height;
    public int capacity;

    public string room;
    public string seats;
}
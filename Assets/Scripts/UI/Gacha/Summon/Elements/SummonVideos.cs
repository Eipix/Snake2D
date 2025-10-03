
public struct SummonVideos
{
    public VideoData Start { get; private set; }
    public VideoData Ray { get; private set; }

    public SummonVideos(string startMP4, string rayMP4)
    {
        Start = new VideoData(startMP4);
        Ray = new VideoData(rayMP4);
    }

    public struct VideoData
    {
        public string FileName { get; private set; }

        public string Url => CachedVideos.GetCachedVideoPath(FileName);

        public VideoData(string fileName) => FileName = fileName;
    }
}

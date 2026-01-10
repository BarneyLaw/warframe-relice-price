using Tesseract;

namespace warframe_relice_price.OCRVision
{
    public static class TesseractObject
    {
        public static readonly TesseractEngine tessEngine = new TesseractEngine(@"./tessdata", "eng", EngineMode.Default);
    }
}

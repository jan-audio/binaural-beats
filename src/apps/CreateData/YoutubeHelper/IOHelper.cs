namespace CreateData.YoutubeHelper
{
    public static class IOHelper
    {
        public static string GetTempPath(string subPath)
        {
            //var basePath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var basePath = Directory.GetCurrentDirectory();
            basePath = Path.Combine(basePath, "App-Data", subPath);
            if (basePath.Split(new[] { '\\', '/' }).LastOrDefault()?.Contains(".") ?? false)
            {
                basePath = Path.GetDirectoryName(basePath);
            }
            if (!Directory.Exists(basePath))
                Directory.CreateDirectory(basePath);
            return basePath;
        }

        //TODO : Đang lỗi tạo file nhầm sang folder
        public static string GenerateTempPath(string subPath)
        {
            while (true)
            {
                var fileName = $"gen-{Guid.NewGuid().ToString().ToLower()}";
                fileName = Path.Combine(subPath, fileName);
                var path = GetTempPath(fileName);
                if (!File.Exists(path))
                    return path;
            }
        }

        public static string GenerateTempFile(string subPath, string extension)
        {
            while (true)
            {
                var fileName = $"gen-{Guid.NewGuid().ToString().ToLower()}{(string.IsNullOrEmpty(extension) ? "" : $".{extension}")}";
                fileName = Path.Combine(subPath, fileName);
                var path = GetTempPath(fileName);
                if (!File.Exists(path))
                    return path;
            }
        }
    }
}

namespace CreateData.YoutubeHelper
{
    public static class IOHelper
    {
        public static string GetTempPath(string subPath)
        {
            //var basePath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var basePath = Directory.GetCurrentDirectory();
            basePath = Path.Combine(basePath, "App-Data", subPath);
            var path = Path.GetDirectoryName(basePath);
            path ??= basePath;
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
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

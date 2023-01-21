using System;

namespace WinUIFileSearchEngine.Models;

public class SearchResult
{
    public string Name { get; set; }
    public DateTime ModifiedDate { get; set; }
    public string Type { get; set; }
    public long Size { get; set; }
    public string Path { get; set; }
}
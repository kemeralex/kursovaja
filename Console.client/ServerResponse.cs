
// Structures for JSON deserialization
public struct ServerResponse
{
    public string message { get; set; }
    public int[]? values { get; set; }
    public int? singleValue { get; set; }
}

public class ResponseForHistory
{
    public string? operation { get; set; } // Type of operation
    public object? parameters { get; set; } // Parameters used in the operation
    public string? result { get; set; } // Result (success/failure)
}


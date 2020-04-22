using System;
using System.Text.Json.Serialization;

public class APODResponse
{
	/* This class has properties that match the properties in a JSON response from the APOD server. 
	 * When a response is made to APOD, the JSON response is deserialized - converted into an APODResponse C# object
	 * The data in each property in the APODResponse comes from the matching JSON response property names. 
	 */


	public String Copyright { get; set; } = "Image credit: NASA";  // default property value

	public String Date { get; set; }

	public String Explanation { get; set; }
	
	public String Hdurl { get; set; } 

	[JsonPropertyName("media_type")]
	public String MediaType { get; set; }

	[JsonPropertyName("service_version")]
	public String ServiceVersion { get; set; }

	public String Title { get; set; }

	[JsonPropertyName("url")]
	public String ImageUrl { get; set; }

	public string FileSavePath { get; set; }

	public override string ToString()
	{
		return $"TITLE: {Title} \n" +
			$"DATE: {Date} \n" +
			$"EXPLANATION: {Explanation} \n" +
			$"CREDIT: {Copyright}\n" +
			$"FILE SAVE PATH: {FileSavePath}";
	}
}

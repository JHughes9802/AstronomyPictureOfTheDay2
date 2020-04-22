using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace APOD
{
    class APOD
    {
        public static APODResponse FetchAPOD(out string errorMessage, DateTime? forDate = null, string imageSavePath = null)
        {
            WebClient client = new WebClient();
         
            using (client)
            {
                // URL for making APIs requests to APOD service, this requests today's picture
                // Try this URL in a browser, you'll see a response in JSON format 
                string url = "https://api.nasa.gov/planetary/apod?api_key=6RVB2VqVPHzfyI9TQ0d4XeQEiAopapJf3RmvapIk";

                try
                {
                    // If a DateTime is provided, format as yyyy-MM-dd for example, 2020-03-30 for March 30th 2020
                    // and append to the URL to request photo for that date 
                    if (forDate != null)
                    {
                        string isoDate = $"{forDate:yyyy-MM-dd}";
                        url += $"&date={isoDate}";
                    }

                    Debug.WriteLine("Using URL " + url);

                    // Make request, download JSON response as a string 
                    var responseString = client.DownloadString(url);

                    // Configure JSON serializer
                    // use setting to convert JSON lowercase attributes to C# style CamelCase variables
                    var serializerOptions = new JsonSerializerOptions
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                    };

                    // Deserialize - convert the JSON to a custom C# object of type APODResponse. 
                    // The properties and values in the JSON are converted to properties and values in the C# object
                    // See also the APODResponse.cs file 
                    APODResponse response = JsonSerializer.Deserialize<APODResponse>(responseString, serializerOptions);

                    // If no image save path set, use the temp directory on the computer, with default filename. 
                    if (imageSavePath == null)
                    {
                        imageSavePath = Path.Combine(Path.GetTempPath(), "apod.jpg");
                    }

                    // The response contains a URL that points to the actual image. Download to the imageSavePath 
                    client.DownloadFile(response.ImageUrl, imageSavePath);

                    // Add the path the image was saved to, to the APODResponse object by setting the FileSavePath property
                    response.FileSavePath = imageSavePath;

                    // For troubleshooting
                    Debug.WriteLine(response);

                    // Everything seems to have worked, set errorMessage to null 
                    // and return response containing all the APOD data
                    errorMessage = null;
                    return response;

                } 
                catch (WebException we)
                {
                    // Catch various connectivity problems
                    errorMessage = "Error fetching data from NASA because " + we.Message;
                }
                catch (IOException ioe)
                {
                    // Catch various image save file problems 
                    errorMessage = "Error saving image file because " + ioe.Message;
                }
                catch (Exception ex)
                {
                    // And for other things that may go wrong. 
                    errorMessage = "Unexpected Exception with message " + ex.Message;
                }

                // For troubleshooting
                Debug.WriteLine($"Error fetching data from APOD server because {errorMessage}");

                return null;   // Caller will be able to check for a null return value, to test if request was not succesful
            }
        }
    }
}

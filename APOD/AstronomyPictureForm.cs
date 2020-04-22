using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace APOD
{
    public partial class AstronomyPictureForm : Form
    {
        public AstronomyPictureForm()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            dtePictureDate.Value = DateTime.Today;
            dtePictureDate.MinDate = new DateTime(1995, 6, 16);
            dtePictureDate.MaxDate = DateTime.Today;

            /* I'd like to have this action only happen if today's picture passes the
             * requirements for a valid picture, but I'm not sure how to do it.
             * Biggest thing is, it's bad to have the user get an error upon
             * opening the program */
            GetAPOD(DateTime.Today); // Changed this to GetAPOD, as shown in slides (Guess I thought a bit outside the box)
        }

        private void btnGetToday_Click(object sender, EventArgs e)
        {
            DateTime currentDate = DateTime.Today;
            GetAPOD(currentDate);
        }

        private void btnGetForDate_Click(object sender, EventArgs e)
        {
            DateTime date = dtePictureDate.Value;

            GetAPOD(date);
        }

        private void GetAPOD(DateTime date)
        {
            ClearForm();
            EnableForm(false);

            if (apodBackgroundWorker.IsBusy == false)
            {
                apodBackgroundWorker.RunWorkerAsync(date);
            }

            else
            {
                MessageBox.Show("Please wait for previous request to complete.");
            }
        }


        private void HandleResponse(APODResponse apodResponse, string error)
        {
            if (error != null)
            {
                MessageBox.Show(error, "Error");
                return;
            }

            if (apodResponse.MediaType == "image")
            {
                LoadImageResponseIntoForm(apodResponse);
            }
            else
            {
                MessageBox.Show($"The response is not an image. Please try another date.", "Sorry!");
            }


        }

        private void LoadImageResponseIntoForm(APODResponse apodResponse)
        {
            lblTitle.Text = apodResponse.Title;

            string imageCredits = apodResponse.Copyright.Replace("\n", " ");
            string lowercaseImageCredits = imageCredits.ToLower();

            if (lowercaseImageCredits.Contains("image credit"))
            {
                lblCredits.Text = apodResponse.Copyright;
            }
            else
            {
                lblCredits.Text = $"Image credit: {imageCredits}";
            }

            DateTime date = DateTime.Parse(apodResponse.Date);

            string formattedDate = $"{date:D}";
            lblDate.Text = formattedDate;

            lblDescription.Text = apodResponse.Explanation;

            try
            {
                picAstronomyPicture.Image = Image.FromFile(apodResponse.FileSavePath);
            }
            catch (Exception e)
            {
                Debug.WriteLine($"Error loading image saved for {apodResponse}\n{e.Message}");
            }
        }


        private void ClearForm()
        {
            lblDate.Text = "";
            lblDescription.Text = "";
            lblTitle.Text = "";
            lblCredits.Text = "";

            picAstronomyPicture.Image?.Dispose();
            picAstronomyPicture.Image = null;
        }


        private void EnableForm(Boolean enable)
        {
            btnGetForDate.Enabled = enable;
            btnGetToday.Enabled = enable;
            dtePictureDate.Enabled = enable;

            progressBar.Visible = !enable;
        }


        private void apodBackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            if (e.Argument is DateTime dt)
            {
                APODResponse apodResponse = APOD.FetchAPOD(out string error, dt);
                e.Result = (reponse: apodResponse, error);
                Debug.WriteLine(e.Result);
            }
            else
            {
                Debug.WriteLine("Background worker error - argument not a DateTime" + e.Argument);
                throw new Exception("Incorrect Argument type, must be a DateTime");
            }
        }

        private void apodBackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                MessageBox.Show($"Unexpected Error fetching data", "Error");
                Debug.WriteLine($"Background Worker error {e.Error}");
            }
            else
            {
                try
                {
                    var (response, error) = ((APODResponse, string))e.Result;

                    HandleResponse(response, error);
                }
                catch (Exception err)
                {
                    Debug.WriteLine($"Unexpected response from APOD request worker: {e.Result} causing error {err}");
                    MessageBox.Show($"Unexpected data returned from APOD request", "Error");
                }
            }

            EnableForm(true);
        }
    }
}
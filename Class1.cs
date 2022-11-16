using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;

namespace SLIC_Services_New
{
    public class Class1
    {
        public HttpResponseMessage Get(string p_polno, string p_end_date, string p_poltype, string p_pmtype, string p_vehNo, string p_timestamp)
        {
            {
                EF_GEN_DAL ef_gen = new EF_GEN_DAL();
                string uName = HttpContext.Current.User.Identity.Name;
                if (ef_gen.check_policy_owner(uName, p_polno)) // @ Jayan Consider this as true
                {
                    int startx = 30;
                    int starty = 90;
                    int gapx = 120;
                    int gapy = 33;

                    IList<CoverDetails> motorCovers = null;

                    MotorPolicyDetails mpData = new MotorPolicyDetails();
                    GeneralDAL da = new GeneralDAL();

                    var list = da.getMotorPolicyList(p_polno, p_end_date, p_poltype, p_pmtype); // GET_MOTOR_RENW_INFO
                    BV_GEN_services bvg = new BV_GEN_services();
                    var coverDet = bvg.getCoverDetails(p_polno);

                    mpData.myMotorPolicyData = list;
                    mpData.myMotorPolicyCover = coverDet;

                    EF_GEN_DAL gen = new EF_GEN_DAL();
                    var res = gen.returnVehicleDetails(p_polno, Convert.ToInt32(mpData.myMotorPolicyData[0].startDt.Substring(0, 4)));

                    string path = System.Web.HttpContext.Current.Request.MapPath(@"~/Images/motorInstCard.png");

                    Image image = Image.FromFile(path); //or .jpg, etc...

                    string address1 = (String.IsNullOrEmpty(mpData.myMotorPolicyData[0].addrss1) ? "" : mpData.myMotorPolicyData[0].addrss1) +
                                      (String.IsNullOrEmpty(mpData.myMotorPolicyData[0].addrss2) ? "." : (mpData.myMotorPolicyData[0].addrss1.Trim().Substring(mpData.myMotorPolicyData[0].addrss1.Trim().Length - 1) == "," ? " " : ",") + mpData.myMotorPolicyData[0].addrss2);

                    string address2 = (String.IsNullOrEmpty(mpData.myMotorPolicyData[0].addrss3) ? "." : (mpData.myMotorPolicyData[0].addrss2.Trim().Substring(mpData.myMotorPolicyData[0].addrss2.Trim().Length - 1) == "," ? "" : ",") + mpData.myMotorPolicyData[0].addrss3) +
                                      (String.IsNullOrEmpty(mpData.myMotorPolicyData[0].addrss4) ? "." : (mpData.myMotorPolicyData[0].addrss3.Trim().Substring(mpData.myMotorPolicyData[0].addrss3.Trim().Length - 1) == "," ? " " : ", ") + mpData.myMotorPolicyData[0].addrss4);

                    //Graphics graphics = Graphics.FromImage(image);
                    using (Graphics grp = Graphics.FromImage(image))
                    {
                        //Set the Color of the Watermark text.
                        Brush brush = new SolidBrush(Color.Black);

                        Brush brush_wtr_mark = new SolidBrush(Color.FromArgb(255, 180, 140));
                        Font font_wtr_mark = new System.Drawing.Font("Arial", 10, FontStyle.Regular, GraphicsUnit.Pixel);

                        //Set the Font and its size.
                        Font font = new System.Drawing.Font("Arial", 16, FontStyle.Regular, GraphicsUnit.Pixel);
                        Font font_Blold = new System.Drawing.Font("Arial", 16, FontStyle.Bold, GraphicsUnit.Pixel);
                        Point position = new Point(startx, starty);

                        for (int i = 60; i < image.Height - 10; i = i + 90)
                        {
                            position = new Point(30, i);
                            string wordign = "This is an electonic print of the insurance card of the policy : " + p_polno + " Vehicle No : " + p_vehNo + ". ";
                            wordign = wordign + wordign;

                            wordign = wordign.Substring(0, 145);

                            grp.DrawString(wordign, font_wtr_mark, brush_wtr_mark, position);
                        }

                        //Determine the size of the Watermark text.
                        //SizeF textSize = new SizeF();
                        //textSize = grp.MeasureString(watermarkText, font);

                        //Position the text and draw it on the image.
                        position = new Point(startx, starty);
                        grp.DrawString("Vehicle No.", font, brush, position);

                        position = new Point(startx + gapx, starty);
                        grp.DrawString(": " + p_vehNo, font_Blold, brush, position);

                        starty = starty + gapy;

                        position = new Point(startx, starty);
                        grp.DrawString("Policy No.", font, brush, position);

                        position = new Point(startx + gapx, starty);
                        grp.DrawString(": " + p_polno, font_Blold, brush, position);

                        starty = starty + gapy;

                        position = new Point(startx, starty);
                        grp.DrawString("Name", font, brush, position);

                        position = new Point(startx + gapx, starty);
                        grp.DrawString(": " + mpData.myMotorPolicyData[0].insurdName, font_Blold, brush, position);

                        starty = starty + gapy;

                        position = new Point(startx, starty);
                        grp.DrawString("NIC/P.P. No", font, brush, position);

                        position = new Point(startx + gapx, starty);
                        grp.DrawString(": " + res.IDNo, font_Blold, brush, position);

                        starty = starty + gapy;

                        position = new Point(startx, starty);
                        grp.DrawString("Address", font, brush, position);

                        position = new Point(startx + gapx, starty);
                        grp.DrawString(": " + address1, font_Blold, brush, position);

                        starty = starty + gapy;

                        position = new Point(startx + gapx, starty);
                        grp.DrawString("  " + address2, font_Blold, brush, position);

                        starty = starty + gapy;

                        position = new Point(startx, starty);
                        grp.DrawString("Period of Cover", font, brush, position);

                        position = new Point(startx + gapx, starty);
                        grp.DrawString(": From " + mpData.myMotorPolicyData[0].startDt + " To " + mpData.myMotorPolicyData[0].endDt, font_Blold, brush, position);

                        starty = starty + gapy;

                        position = new Point(startx, starty);
                        grp.DrawString("Engine No", font, brush, position);

                        position = new Point(startx + gapx, starty);
                        grp.DrawString(": " + res.EngineNo, font_Blold, brush, position);

                        starty = starty + gapy;

                        position = new Point(startx, starty);
                        grp.DrawString("Chassis No", font, brush, position);

                        position = new Point(startx + gapx, starty);
                        grp.DrawString(": " + res.ChassisNo, font_Blold, brush, position);

                        image.RotateFlip(RotateFlipType.Rotate270FlipNone);

                        using (MemoryStream memoryStream = new MemoryStream())
                        {
                            //Save the Watermarked image to the MemoryStream.

                            image.Save(memoryStream, ImageFormat.Png);
                            memoryStream.Position = 0;

                            HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
                            result.Content = new ByteArrayContent(memoryStream.ToArray());
                            result.Content.Headers.ContentType = new MediaTypeHeaderValue("image/png");

                            return result;
                        }
                    }
                }
                else
                {
                    HttpResponseMessage result1 = new HttpResponseMessage(HttpStatusCode.NotFound);
                    return result1;
                }

            }
        }
}
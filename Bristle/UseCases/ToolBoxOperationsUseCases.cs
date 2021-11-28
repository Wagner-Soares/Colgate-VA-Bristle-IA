using APIVision.Controllers;
using Bristle.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bristle.UseCases
{
    static class ToolBoxOperationsUseCases
    {
        public static void DeleteBoundBoxFromPrediction(AutomaticBristleClassification automaticBristleClassification, int boundingBoxToRemove)
        {
            if (boundingBoxToRemove > -1)
            {
                automaticBristleClassification.SocketModels[boundingBoxToRemove].Obj_class = "discard";
                automaticBristleClassification.businessSystem.BoundingBoxDiscards.Add(automaticBristleClassification.SocketModels[boundingBoxToRemove]);
                automaticBristleClassification.SocketModels.RemoveAt(boundingBoxToRemove);
                automaticBristleClassification.BoundingBoxSelect = 0;
                automaticBristleClassification.SelectedCanvasBoundingBox = -1;
            }  
        }

        public static void ClassifyBristleAsError1(AutomaticBristleClassification automaticBristleClassification, int selectedSocketModel)
        {
            if (selectedSocketModel != -1)
            {
                automaticBristleClassification.BoundingBoxSelectType = "Error1";
                automaticBristleClassification.ChangeClassInPlace(selectedSocketModel);
            }
        }

        public static void ClassifyBristleAsError2(AutomaticBristleClassification automaticBristleClassification, int selectedSocketModel)
        {
            if (selectedSocketModel != -1)
            {
                automaticBristleClassification.BoundingBoxSelectType = "Error2";
                automaticBristleClassification.ChangeClassInPlace(selectedSocketModel);
            }
        }

        public static void ClassifyBristleAsError3(AutomaticBristleClassification automaticBristleClassification, int selectedSocketModel)
        {
            if (selectedSocketModel != -1)
            {
                automaticBristleClassification.BoundingBoxSelectType = "Error3";
                automaticBristleClassification.ChangeClassInPlace(selectedSocketModel);
            }
        }

        public static void ClassifyBristleAsOk(AutomaticBristleClassification automaticBristleClassification, int selectedSocketModel)
        {
            if (selectedSocketModel != -1)
            {
                automaticBristleClassification.BoundingBoxSelectType = "Ok";
                automaticBristleClassification.ChangeClassInPlace(selectedSocketModel);
            }
        }

        public static void ResizeBristleToSize1(AutomaticBristleClassification automaticBristleClassification, int selectedSocketModel)
        {
            if (selectedSocketModel != -1)
            {
                ResizeBox(automaticBristleClassification,100);
                automaticBristleClassification.BoundingBoxSelectType = automaticBristleClassification.SocketModels[automaticBristleClassification.SelectedCanvasBoundingBox - 1].Obj_class;
                automaticBristleClassification.ChangeClassInPlace(selectedSocketModel);
            }
        }

        public static void ResizeBristleToSize2(AutomaticBristleClassification automaticBristleClassification, int selectedSocketModel)
        {
            if (selectedSocketModel != -1)
            {
                ResizeBox(automaticBristleClassification,200);
                automaticBristleClassification.BoundingBoxSelectType = automaticBristleClassification.SocketModels[automaticBristleClassification.SelectedCanvasBoundingBox - 1].Obj_class;
                automaticBristleClassification.ChangeClassInPlace(selectedSocketModel);
            }
        }

        public static void ResizeBristleToSize3(AutomaticBristleClassification automaticBristleClassification, int selectedSocketModel)
        {
            if (automaticBristleClassification.SelectedCanvasBoundingBox != -1)
            {
                ResizeBox(automaticBristleClassification,300);
                automaticBristleClassification.BoundingBoxSelectType = automaticBristleClassification.SocketModels[automaticBristleClassification.SelectedCanvasBoundingBox - 1].Obj_class;
                automaticBristleClassification.ChangeClassInPlace(selectedSocketModel);
            }
        }

        public static void ResizeBox(AutomaticBristleClassification automaticBristleClassification,int sizeForSocketModel)
        {
            if (automaticBristleClassification.SelectedCanvasBoundingBox > -1)
            {
                try
                {
                    var SocketModel = automaticBristleClassification.SocketModels[automaticBristleClassification.SelectedCanvasBoundingBox - 1];
                    automaticBristleClassification.SocketModels.RemoveAt(automaticBristleClassification.SelectedCanvasBoundingBox - 1);
                    SocketModel.Probability = -1;

                    SocketModel.X += SocketModel.Width / 2;
                    SocketModel.X -= sizeForSocketModel / 2;
                    SocketModel.Y += SocketModel.Height / 2;
                    SocketModel.Y -= sizeForSocketModel / 2;

                    SocketModel.Height = sizeForSocketModel;
                    SocketModel.Width = sizeForSocketModel;
                    automaticBristleClassification.SocketModels.Add(SocketModel);
                    automaticBristleClassification.DrawBoundingBox();
                    automaticBristleClassification.BoundingBoxSelect = 0;
                    automaticBristleClassification.SelectedCanvasBoundingBox = -1;
                    automaticBristleClassification.BoundingBoxSelectType = "undefined";
                }
                catch
                {
                    //tryCatch to avoid crash
                }
            }
        }
    }
}

using APIVision.Controllers;
using APIVision.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bristle.UseCases
{
    public static class AutomaticBristleClassificationUseCases
    {
        public static SocketModel SocketModelScaleConversion(SocketModel givenSocketModel, double maxWidth, double maxHeight, string classToAtribute)
        {
            givenSocketModel.Width = (int)((givenSocketModel.Width * CameraObject.ResolutionWidth) / maxWidth);
            givenSocketModel.Height = (int)((givenSocketModel.Height * CameraObject.ResolutionHeight) / maxHeight);
            givenSocketModel.X = (int)((givenSocketModel.X * CameraObject.ResolutionWidth) / maxWidth);
            givenSocketModel.Y = (int)((givenSocketModel.Y * CameraObject.ResolutionHeight) / maxHeight);
            givenSocketModel.Obj_class = DataHandlerUseCases.ConvertIAToDefault(classToAtribute);
            givenSocketModel.Probability = -1;

            return givenSocketModel;
        }
    }
}

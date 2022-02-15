using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevitRoomPlugin
{
    [TransactionAttribute(TransactionMode.Manual)]
    public class Command : IExternalCommand
    {

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {

            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;


            List<ViewPlan> viewPlans = new FilteredElementCollector(doc)
                   .OfClass(typeof(ViewPlan))
                   .Cast<ViewPlan>()
                   .ToList();


            FilteredElementCollector rooms = new FilteredElementCollector(doc, doc.ActiveView.Id)
                   .OfCategory(BuiltInCategory.OST_Rooms);


            List<FamilySymbol> familySymbol = new FilteredElementCollector(doc)
                            .OfClass(typeof(FamilySymbol))
                            .OfCategory(BuiltInCategory.OST_RoomTags)
                            .Cast<FamilySymbol>()
                            .ToList();
            View view = doc.ActiveView;
            IList<ElementId> roomId = rooms.ToElementIds() as List<ElementId>;
           // TaskDialog.Show("Количество помещений", roomId.Count().ToString());

            Transaction tr = new Transaction(doc, "Numbering of fooms");
            tr.Start();

            //установка марки в центр границы помещения
           /* foreach (ElementId r in roomId)
            {
                Room oRoom = doc.GetElement(r) as Room;
                XYZ roomCenter = GetElementCenter(oRoom);
                UV center = new UV(roomCenter.X, roomCenter.Y);
                LinkElementId rId = new LinkElementId(oRoom.Id);
                doc.Create.NewRoomTag(rId, center, view.Id);
            }*/


            //установка марки в точку помещения

            foreach (ElementId r in roomId)
            {
                Room oRoom = doc.GetElement(r) as Room;

                LocationPoint rlp = oRoom.Location as LocationPoint;

                LinkElementId rId = new LinkElementId(oRoom.Id);

                XYZ p1 = rlp.Point;

                UV center = new UV(p1.X, p1.Y);

                doc.Create.NewRoomTag(rId, center, view.Id);
            }
            tr.Commit();

            return Result.Succeeded;
        }

        public XYZ GetElementCenter(Element element)
        {
            BoundingBoxXYZ bounding = element.get_BoundingBox(null);
            return (bounding.Max + bounding.Min) / 2;
        }

    }
}

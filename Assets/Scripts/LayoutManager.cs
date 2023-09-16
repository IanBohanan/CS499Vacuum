using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;

#region Object Structs
struct Wall
{
    string orientation;
    int xPos;
    int yPos;
    public Wall(string orientation, int xPos, int yPos)
    {
        this.orientation = orientation;
        this.xPos = xPos;
        this.yPos = yPos;
    }
};
struct RoomDoor
{
    string orientation;
    int xPos;
    int yPos;
    public RoomDoor(string orientation, int xPos, int yPos)
    {
        this.orientation = orientation;
        this.xPos = xPos;
        this.yPos = yPos;
    }
};
struct ExitDoor
{
    string orientation;
    int xPos;
    int yPos;
    public ExitDoor(string orientation, int xPos, int yPos)
    {
        this.orientation = orientation;
        this.xPos = xPos;
        this.yPos = yPos;
    }
};
struct Furniture
{
    string type;
    int xPos;
    int yPos;
    public Furniture(string type, int xPos, int yPos)
    {
        this.type = type;
        this.xPos = xPos;
        this.yPos = yPos;
    }
};
#endregion

public class LayoutManager
{
    List<Wall> walls = new List<Wall>();
    List<RoomDoor> roomDoors = new List<RoomDoor>();
    List<ExitDoor> exitDoors = new List<ExitDoor>();
    List<Furniture> furniture = new List<Furniture>();

    #region List Add Functions
    public void addWall(string orientation, int xPos, int yPos)
    {
        walls.Add(new Wall(orientation, xPos, yPos));
    }
    public void addRoomDoor(string orientation, int xPos, int yPos)
    {
        roomDoors.Add(new RoomDoor(orientation, xPos, yPos));
    }
    public void addExitDoor(string orientation, int xPos, int yPos)
    {
        exitDoors.Add(new ExitDoor(orientation, xPos, yPos));
    }
    public void addFurniture(string type, int xPos, int yPos)
    {
        furniture.Add(new Furniture(type, xPos, yPos));
    }
    #endregion

    #region List Delete Functions
    public bool deleteWall(){
        return false; // Failed to remove
        return true; // Successfully removed
    }
    #endregion
}
namespace WestCoastEducation.Interfaces;

// Allt som kan presenteras som en sträng med intressant information.
// Gör att samma listmetod kan användas för både kurser och personer.

public interface IPresentable
{
    string Present();
}

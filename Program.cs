namespace Scout;

internal class Program
{
    private static void Main()
    {
        using var reader = new PBFReader("../scout/assets/luxembourg.osm.pbf");
        reader.Open();
        reader.ReadNext();
    }
}

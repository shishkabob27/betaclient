using System.Text;

public class MetadataDictionary
{
    private readonly Dictionary<byte, MetadataEntry> entries;

    public MetadataDictionary()
    {
        entries = new Dictionary<byte, MetadataEntry>();
    }

    public int Count
    {
        get { return entries.Count; }
    }

    public MetadataEntry this[byte index]
    {
        get { return entries[index]; }
        set { entries[index] = value; }
    }

    public static MetadataDictionary FromStream(MinecraftStream stream)
    {
        var value = new MetadataDictionary();
        while (true)
        {
            byte key = stream.ReadUInt8();
            if (key == 127) break;

            //byte type = (byte)((key & 0xE0) >> 5);
            //byte index = (byte)(key & 0x1F);

            //var entry = EntryTypes[type]();
            ///entry.FromStream(stream);
            //entry.Index = index;

            //value[index] = entry;
        }
        return value;
    }

    public void WriteTo(MinecraftStream stream)
    {
        foreach (var entry in entries)
            entry.Value.WriteTo(stream, entry.Key);
        stream.WriteUInt8(0x7F);
    }

    delegate MetadataEntry CreateEntryInstance();

    private static readonly CreateEntryInstance[] EntryTypes = new CreateEntryInstance[]
        {
            () => new MetadataByte(), // 0
            () => new MetadataShort(), // 1
            () => new MetadataInt(), // 2
            () => new MetadataFloat(), // 3
            () => new MetadataString(), // 4
            () => new MetadataSlot(), // 5
        };

    public override string ToString()
    {
        if (entries.Count == 0)
            return String.Empty;

        StringBuilder sb = new StringBuilder();

        foreach (var entry in entries.Values)
        {
            sb.Append(entry.ToString());
            sb.Append(", ");
        }

        sb.Length -= 2;

        return sb.ToString();
    }
}
namespace BL.Domain.Test
{
    public class TestTag
    {
        public TestTag()
        {
        }

        public TestTag(Tag tag)
        {
            Tag = tag;
        }

        public int Id { get; set; }

        public Test Test { get; set; }
        public Tag Tag { get; set; }
    }
}
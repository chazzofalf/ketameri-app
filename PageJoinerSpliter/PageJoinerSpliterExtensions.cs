namespace PageJoinerSpliter;

public static class PageJoinerSpliterExtensions
{
    public static void Deconstruct(this int[] array,out int top,out int bottom,out int right,out int left)
    {
        top = array[0];
        bottom = array[1];
        right = array[2];
        left = array[3];
    }
}

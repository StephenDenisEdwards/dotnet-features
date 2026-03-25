Console.WriteLine(SumArray(1, 2, 3));
Console.WriteLine(SumSpan(1, 2, 3));

static int SumArray(params int[] values)
{
    int sum = 0;
    foreach (var v in values) sum += v;
    return sum;
}

static int SumSpan(params ReadOnlySpan<int> values)
{
    int sum = 0;
    foreach (var v in values) sum += v;
    return sum;
}

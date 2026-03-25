using System.Diagnostics;

// ---------------------------------------------------------------------------
// 1. Create an ActivitySource
// ---------------------------------------------------------------------------

var source = new ActivitySource("DemoApp", "1.0.0");

// ---------------------------------------------------------------------------
// 2. Set up an ActivityListener that samples everything and prints on stop
// ---------------------------------------------------------------------------

var listener = new ActivityListener
{
    ShouldListenTo = _ => true,
    Sample = (ref ActivityCreationOptions<ActivityContext> _) => ActivitySamplingResult.AllDataAndRecorded,
    ActivityStopped = activity =>
    {
        var depth = 0;
        var parent = activity.Parent;
        while (parent is not null)
        {
            depth++;
            parent = parent.Parent;
        }
        var indent = new string(' ', depth * 2);

        Console.WriteLine($"{indent}[Stopped] {activity.OperationName}");
        Console.WriteLine($"{indent}  TraceId  : {activity.TraceId}");
        Console.WriteLine($"{indent}  SpanId   : {activity.SpanId}");
        Console.WriteLine($"{indent}  ParentId : {activity.ParentSpanId}");
        Console.WriteLine($"{indent}  Duration : {activity.Duration.TotalMilliseconds:F1}ms");

        if (activity.Tags.Any())
        {
            Console.WriteLine($"{indent}  Tags     :");
            foreach (var (key, value) in activity.Tags)
                Console.WriteLine($"{indent}    {key} = {value}");
        }

        if (activity.Events.Any())
        {
            Console.WriteLine($"{indent}  Events   :");
            foreach (var evt in activity.Events)
            {
                Console.Write($"{indent}    - {evt.Name}");
                if (evt.Tags.Any())
                    Console.Write($" ({string.Join(", ", evt.Tags.Select(t => $"{t.Key}={t.Value}"))})");
                Console.WriteLine();
            }
        }

        if (activity.Baggage.Any())
        {
            Console.WriteLine($"{indent}  Baggage  :");
            foreach (var (key, value) in activity.Baggage)
                Console.WriteLine($"{indent}    {key} = {value}");
        }

        Console.WriteLine();
    }
};

ActivitySource.AddActivityListener(listener);

// ---------------------------------------------------------------------------
// 3-7. Create activities with parent-child relationships, tags, events, baggage
// ---------------------------------------------------------------------------

Console.WriteLine("=== Distributed Tracing Demo ===");
Console.WriteLine();

// Parent activity: ProcessOrder
using (var processOrder = source.StartActivity("ProcessOrder", ActivityKind.Internal))
{
    if (processOrder is not null)
    {
        // Add tags to parent
        processOrder.SetTag("order.id", "ORD-12345");
        processOrder.SetTag("customer.name", "Alice Johnson");

        // Set baggage on parent — propagates to children
        processOrder.SetBaggage("tenant.id", "tenant-42");
        processOrder.SetBaggage("region", "us-east-1");

        // Child activity: ValidateOrder
        using (var validate = source.StartActivity("ValidateOrder", ActivityKind.Internal))
        {
            if (validate is not null)
            {
                validate.SetTag("validation.rules", "3");
                validate.SetTag("validation.result", "passed");
                validate.AddEvent(new ActivityEvent("RulesEvaluated",
                    tags: new ActivityTagsCollection
                    {
                        { "rules.checked", 3 },
                        { "rules.passed", 3 }
                    }));

                // Read baggage set on parent
                var tenantId = validate.GetBaggageItem("tenant.id");
                Console.WriteLine($"[ValidateOrder] Read parent baggage tenant.id = {tenantId}");
            }

            // Simulate work
            await Task.Delay(50);
        }

        // Child activity: ChargePayment
        using (var charge = source.StartActivity("ChargePayment", ActivityKind.Internal))
        {
            if (charge is not null)
            {
                charge.SetTag("payment.method", "credit_card");
                charge.SetTag("payment.amount", "99.95");
                charge.SetTag("payment.currency", "USD");

                // Simulate processing
                await Task.Delay(80);

                charge.AddEvent(new ActivityEvent("PaymentProcessed",
                    tags: new ActivityTagsCollection
                    {
                        { "transaction.id", "TXN-98765" },
                        { "processor", "Stripe" }
                    }));

                charge.AddEvent(new ActivityEvent("ReceiptGenerated"));

                // Read baggage set on parent
                var region = charge.GetBaggageItem("region");
                Console.WriteLine($"[ChargePayment] Read parent baggage region = {region}");
            }
        }

        processOrder.AddEvent(new ActivityEvent("OrderCompleted"));
    }
}

// ---------------------------------------------------------------------------
// Summary
// ---------------------------------------------------------------------------

Console.WriteLine("--- Trace Summary ---");
Console.WriteLine("Activities are printed above as they stop (children before parents).");
Console.WriteLine("The TraceId is shared across the entire operation, while each span has");
Console.WriteLine("a unique SpanId. ParentSpanId links children back to their parent.");
Console.WriteLine();
Console.WriteLine("Done.");

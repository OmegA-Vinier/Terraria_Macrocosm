using Macrocosm.Common.Systems.Connectors;

namespace Macrocosm.Content.Items.Connectors;

public class ConveyorWrenchRed : ConveyorPipeWrench
{
    public override ConveyorPipeType PipeType => ConveyorPipeType.RedPipe;
}

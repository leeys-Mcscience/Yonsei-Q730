using McQLib.Core;

namespace Q730.UserControls.Graphs
{
    interface IGraphControl
    {
        void AddData( MeasureData data );

        void AddData(MeasureData data, McQLib.Device.Channel channel);
        void RefreshGraph();
        void ClearGraph();
    }
}

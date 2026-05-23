using UnityEngine;

public class SnapSocketGizmos
{
    private readonly SnapSocket socket;

    public SnapSocketGizmos(
        SnapSocket socket)
    {
        this.socket = socket;
    }

    public void Draw()
    {
        if (socket == null ||
            socket.Point == null)
        {
            return;
        }

        Gizmos.DrawWireSphere(
            socket.Point.position,
            0.045f
        );

        Gizmos.DrawLine(
            socket.Point.position,
            socket.Point.position +
            socket.Point.up * 0.2f
        );
    }
}
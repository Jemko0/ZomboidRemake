namespace Iso.Engine.Core.UI
{
    public enum UIEvent
    {
        NULL,

        UPDATE,

        MOUSE_WHEEL,

        MOUSE_LMBCLICK,
        MOUSE_RMBCLICK,
        MOUSE_MMBCLICK,

        MOUSE_LMBRELEASE,
        MOUSE_RMBRELEASE,
        MOUSE_MMBRELEASE,

        MOUSE_MOVE,

        DRAG_START,
        DRAG_MOVE,
        DRAG_STOP,
    }

    public enum UIVisibilityMode
    {
        VISIBLE,
        VISIBLE_NO_HIT_TEST,
        HIDDEN,
        HIDDEN_NO_HIT_TEST,
        COLLAPSED
    }
}

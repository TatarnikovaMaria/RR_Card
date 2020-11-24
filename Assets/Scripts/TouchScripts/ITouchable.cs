public interface ITouchable{

    void OnTouchDown(object[] arg);

    void OnTouchExit(object[] arg);

    void OnTouchUp(object[] arg);

    void OnTouchStay(object[] arg);

    void OnTouchMove(object[] arg);
}
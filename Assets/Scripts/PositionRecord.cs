using UnityEngine;

public class PositionRecord {
  public long timestamp;
  public Vector3 position;
  public PositionRecord(long _timestamp, Vector3 _position) {
    timestamp = _timestamp;
    position = _position;
  }
}

using Unity.Netcode;
using UnityEngine;

namespace NetworkTest
{
    public class Player2 : NetworkBehaviour
    {
        public NetworkVariable<Vector3> position = new NetworkVariable<Vector3>();
        public NetworkVariable<Vector2> movementInput = new NetworkVariable<Vector2>(
            NetworkVariableReadPermission.OwnerOnly
        );
        public readonly float movementSpeed = 1f; 
        private PlayerInput controls;

        private void Awake() {
            controls = new PlayerInput();
        }

        private void OnEnable() {
            controls.Enable();
        }

        public override void OnNetworkSpawn()
        {
            if (NetworkManager.Singleton.IsServer)
            {
                var randomPosition = GetRandomPositionOnPlane();
                transform.position = randomPosition;
                position.Value = randomPosition;
            }
        }

        static Vector3 GetRandomPositionOnPlane()
        {
            return new Vector3(Random.Range(-3f, 3f), 1f, Random.Range(-3f, 3f));
        }

        [ServerRpc]
        void MovementServerRpc(Vector2 v) {
            movementInput.Value = controls.movement.move.ReadValue<Vector2>();
        }

        void Update()
        {
            if (NetworkManager.Singleton.IsServer)
            {
                var mvt = movementInput.Value.normalized * movementSpeed * Time.deltaTime;
                position.Value += new Vector3(mvt.x, 0, mvt.y);
            }
            if (NetworkManager.Singleton.IsClient) {
                MovementServerRpc(controls.movement.move.ReadValue<Vector2>());
            }

            transform.position = position.Value;
        }
    }
}
using UnityEngine;
using Unity.Netcode;

public class CopyRotation : MonoBehaviour
{
    public GameObject sourceObject;  // Objek sumber yang akan di-copy rotasinya
    public GameObject horizontalTarget;  // Objek target yang akan mengikuti rotasi horizontal (sumbu Y)
    public GameObject verticalTarget;    // Objek target yang akan mengikuti rotasi vertikal (sumbu X)

    void Update()
    {
        
        if (sourceObject != null)
        {
            // Ambil rotasi dari objek sumber
            Quaternion sourceRotation = sourceObject.transform.rotation;
            Vector3 sourceEuler = sourceRotation.eulerAngles;

            // Jika target horizontal tidak null, salin hanya rotasi horizontal (sumbu Y)
            if (horizontalTarget != null)
            {
                Vector3 horizontalEuler = horizontalTarget.transform.rotation.eulerAngles;
                horizontalEuler.y = sourceEuler.y;  // Salin rotasi sumbu Y dari source
                horizontalTarget.transform.rotation = Quaternion.Euler(horizontalEuler); // Terapkan rotasi baru
            }

            // Jika target vertikal tidak null, salin hanya rotasi vertikal (sumbu X)
            if (verticalTarget != null)
            {
                Vector3 verticalEuler = verticalTarget.transform.rotation.eulerAngles;
                verticalEuler.x = sourceEuler.x;  // Salin rotasi sumbu X dari source
                verticalTarget.transform.rotation = Quaternion.Euler(verticalEuler); // Terapkan rotasi baru
            }
        }
    }
}

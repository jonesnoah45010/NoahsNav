using UnityEngine;
using UnityEngine.UI;

public class NoahsNavUI : MonoBehaviour
{
    public Button quitButton;


    private void Awake()
    {
        // Ensure the button is assigned
        if (quitButton == null)
        {
            Debug.LogError("QuitButton is not assigned in the Inspector!");
            return;
        }

        // Register the QuitApplication method to the button's onClick event
        quitButton.onClick.AddListener(QuitApplication);
    }
    public void QuitApplication()
    {
        // Print a message to the console (useful for testing in the editor).
        Debug.Log("Quit button pressed!");

        // If the application is running in the editor, this won't work.
        // Use Application.Quit() to quit the built application.
        Application.Quit();
    }
}

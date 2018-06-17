using UnityEngine;
using System.Text;
using System.Collections;

namespace Kudan.AR
{
    [AddComponentMenu("Kudan AR/Transform Drivers/Markerless Driver")]
	/// <summary>
	/// The Markerless Transform Driver, which is moved by the tracker when using the Markerless Tracking Method.
	/// </summary>
	public class MarkerlessTransformDriver : TransformDriverBase
	{
        public bool flagRotate = false, flagScale=false;
        private Quaternion startRotate1, quat;
        private float val, beginMousePos;
        private Vector3 vec;
        /// <summary>
        /// Reference to the Markerless Tracking Method.
        /// </summary>
        private TrackingMethodMarkerless _tracker;

		/// <summary>
		/// Finds the tracker.
		/// </summary>
		protected override void FindTracker()
		{
			_trackerBase = _tracker = (TrackingMethodMarkerless)Object.FindObjectOfType(typeof(TrackingMethodMarkerless));
		}

		/// <summary>
		/// Register this instance with the Tracking Method class for event handling.
		/// </summary>
		protected override void Register()
		{
			if (_tracker != null)
			{
				_tracker._updateMarkerEvent.AddListener(OnTrackingUpdate);
			}
            this.gameObject.SetActive(false);

		}

		/// <summary>
		/// Unregister this instance with the Tracking Method class for event handling.
		/// </summary>
		protected override void Unregister()
		{
			if (_tracker != null)
			{
				_tracker._updateMarkerEvent.RemoveListener(OnTrackingUpdate);
			}
		}

        new void Update()
        {
            if (flagRotate)
                if (Input.GetMouseButtonDown(0))
                    beginMousePos = Input.mousePosition.x;
                else
                    if (Input.GetMouseButton(0))
                        quat = Quaternion.AngleAxis(beginMousePos-Input.mousePosition.x, Vector3.up);
            if (flagScale)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    vec = this.transform.localScale;
                    beginMousePos = Input.mousePosition.y / 10;
                }
                else
                    if (Input.GetMouseButton(0))
                    {
                        Debug.Log(Input.mousePosition.y);
                        val = Input.mousePosition.y / 10 - beginMousePos;
                        if (vec.x + val > 0)
                            this.transform.localScale = vec + new Vector3(val, val, val);
                    }
            }
        }

        public void ClickOnRotate()
        {
            if (flagRotate)
                flagRotate = false;
            else
            {
                flagScale = false;
                flagRotate = true;
            }
        }

        public void ClickOnScale()
        {
            if (flagScale)
                flagScale = false;
            else
            {
                flagRotate = false;
                flagScale = true;
            }
        }


        /// <summary>
        /// Method called every frame ArbiTrack is running.
        /// Updates the position and orientation of the trackable.
        /// </summary>
        /// <param name="trackable">Trackable.</param>
        public void OnTrackingUpdate(Trackable trackable)
		{
            this.transform.localPosition = trackable.position;
            this.transform.localRotation = trackable.orientation*quat;

            this.gameObject.SetActive(trackable.isDetected);
		}
	}
};
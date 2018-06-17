using UnityEngine;
using System.Text;
using System.Collections;

namespace Kudan.AR
{
	/// <summary>
	/// The Marker Transform Driver, which is moved by the tracker when using the Marker Tracking Method.
	/// </summary>
	[AddComponentMenu("Kudan AR/Transform Drivers/Marker Based Driver")]
	public class MarkerTransformDriver : TransformDriverBase
	{
        public bool flagRotate = false, flagScale = false;
        private Quaternion startRotate1, quat;
        private float val, beginMousePos;
        private Vector3 vec;
        /// <summary>
        /// Constant scale factor for resizing markers.
        /// </summary>
        private const float UnityScaleFactor = 10f;

		[Tooltip("Optional ID")]
		/// <summary>
		/// The ID of the marker needed to activate this transform driver.
		/// </summary>
		public string _expectedId;

		/// <summary>
		/// Whether or not to resize child objects of this transform driver.//Применять масштабирование
		/// </summary>
		public bool _applyMarkerScale;

		[Header("Plane Drawing")]
		/// <summary>
		/// Whether or not to draw a box showing the space the marker takes up in the virtual world. //Рисовать ящик или нет
		/// </summary>
		public bool _alwaysDrawMarkerPlane = true;

		/// <summary>
		/// The width of the marker plane.
		/// </summary>
		public int _markerPlaneWidth;

		/// <summary>
		/// The height of the marker plane.
		/// </summary>
		public int _markerPlaneHeight;

		/// <summary>
		/// The ID of a detected trackable.
		/// </summary>
		private string _trackableId;

		/// <summary>
		/// Reference to the Marker Tracking Method.
		/// </summary>
		private TrackingMethodMarker _tracker;

		/// <summary>
		/// Finds the tracker.
		/// </summary>
		protected override void FindTracker()
		{
			_trackerBase = _tracker = (TrackingMethodMarker)Object.FindObjectOfType(typeof(TrackingMethodMarker));
		}

		/// <summary>
		/// Register this instance with the Tracking Method class for event handling.
		/// </summary>
		protected override void Register()
		{
			if (_tracker != null)
			{
				_tracker._foundMarkerEvent.AddListener(OnTrackingFound);
				_tracker._lostMarkerEvent.AddListener(OnTrackingLost);
				_tracker._updateMarkerEvent.AddListener(OnTrackingUpdate);

				this.gameObject.SetActive(false);
			}
		}

		/// <summary>
		/// Unregister this instance with the Tracking Method class for event handling.
		/// </summary>
		protected override void Unregister()
		{
			if (_tracker != null)
			{
				_tracker._foundMarkerEvent.RemoveListener(OnTrackingFound);
				_tracker._lostMarkerEvent.RemoveListener(OnTrackingLost);
				_tracker._updateMarkerEvent.RemoveListener(OnTrackingUpdate);
			}
		}

		/// <summary>
		/// Method called when a marker has been found.
		/// Checks whether the detected marker is the correct one by checking it against the expected ID.
		/// </summary>
		/// <param name="trackable">Trackable.</param>
		public void OnTrackingFound(Trackable trackable)
		{
			bool matches = false;
			if (_expectedId == trackable.name)
			{
				matches = true;
			}

			if (matches)
			{
				_trackableId = trackable.name;
				this.gameObject.SetActive(true);
			}
		}

		/// <summary>
		/// Method called when a marker has been lost.
		/// Checks whether the detected marker is the correct one by checking it against the expected ID.
		/// </summary>
		/// <param name="trackable">Trackable.</param>
		public void OnTrackingLost(Trackable trackable)
		{
			if (_trackableId == trackable.name)
			{
				this.gameObject.SetActive(false);
				_trackableId = string.Empty;
			}
		}

        new void Update()
        {
            if (flagRotate)
                if (Input.GetMouseButtonDown(0))
                    beginMousePos = Input.mousePosition.x;
                else
                    if (Input.GetMouseButton(0))
                    quat = Quaternion.AngleAxis(beginMousePos - Input.mousePosition.x, Vector3.up);
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

        /// <summary>
        /// Method called every frame the marker has been tracked.
        /// Updates the position and orientation of the trackable.
        /// </summary>
        /// <param name="trackable">Trackable.</param>
        public void OnTrackingUpdate(Trackable trackable)
		{
			if (_trackableId == trackable.name)
			{
				if (hasInvertedCamera()) 
				{
					this.transform.localPosition = new Vector3 (-trackable.position.x, -trackable.position.y, trackable.position.z);
				}

				else 
				{
					this.transform.localPosition = trackable.position;
				}

				this.transform.localRotation = trackable.orientation*quat;

				if (_applyMarkerScale)
				{
					this.transform.localScale = new Vector3(trackable.height / UnityScaleFactor, 1f, trackable.width / UnityScaleFactor);
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

        bool hasInvertedCamera()
		{
			if (SystemInfo.deviceModel == "LGE Nexus 5X")
            {
				return true;
            }

            return false;
		}

#if UNITY_EDITOR
		/// <summary>
		/// Sets the scale of child objects using the marker size.
		/// </summary>
		public void SetScaleFromMarkerSize()
		{
			if (_markerPlaneWidth > 0 && _markerPlaneHeight > 0)
			{
				this.transform.localScale = new Vector3(_markerPlaneHeight / UnityScaleFactor, 1f, _markerPlaneWidth / UnityScaleFactor);
			}
		}

		/// <summary>
		/// Draw gizmos for this object only if it is selected.
		/// </summary>
		void OnDrawGizmosSelected()
		{
			DrawPlane();
		}

		/// <summary>
		/// Draw gizmos for this object all the time if it has been set to always draw.
		/// </summary>
		void OnDrawGizmos()
		{
			if (_alwaysDrawMarkerPlane)
			{
				DrawPlane();
			}
		}

		/// <summary>
		/// Draws the marker preview plane.
		/// </summary>
		private void DrawPlane()
		{
			Gizmos.matrix = this.transform.localToWorldMatrix;

			Vector3 size = Vector3.one * UnityScaleFactor;

			// In the editor mode use the user entered size values
			if (!Application.isPlaying)
			{
				if (_markerPlaneWidth > 0 && _markerPlaneHeight > 0)
				{
					size = new Vector3(_markerPlaneHeight, 0.01f, _markerPlaneWidth);
				}
				else
				{
					return;
				}
			}

			// Draw a flat cube to represent the area the marker would take up
			Gizmos.color = Color.magenta;
			Gizmos.DrawWireCube(Vector3.zero, size);
		}
#endif
	}
};

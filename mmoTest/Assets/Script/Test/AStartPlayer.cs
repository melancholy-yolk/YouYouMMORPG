using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

[RequireComponent(typeof(Seeker))]
[RequireComponent(typeof(FunnelModifier))]
public class AStartPlayer : MonoBehaviour {

    private Seeker seeker;
    public ABPath AStarPath;
    public float speed = 100;
    public float nextWaypointDistance = 3;
    private int AStarCurrentWaypoint = 0;

    private float roleSlerp = 0f;

    private CharacterController m_CharacterController;

    public AstarPath AstarPathController;

	void Start () {
        seeker = GetComponent<Seeker>();
        m_CharacterController = GetComponent<CharacterController>();
	}
	
	
	void Update () {
        //��ҵ�������ƶ�
        if (Input.GetMouseButtonDown(0))
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))//�ж�Ҫ�ƶ�����Ŀ���
            {
                Debug.Log(hit.collider.gameObject.name);
                this.AstarGotoPos(hit.point);
            }
        }

        if (!this.m_CharacterController.isGrounded)
        {
            this.m_CharacterController.Move(new Vector3(gameObject.transform.position.x, -1000, gameObject.transform.position.z));
        }
        PlayerAStarMove();
	}

    public void AstarGotoPos(Vector3 AStarTargetPos)
    {
        if (this.seeker != null)
        {
            //��ʼA*Ѱ·���㣬�������ʱ����ص�OnPathComplete����
            this.seeker.StartPath(this.transform.position, AStarTargetPos, OnPathComplete);
        }
    }

    public void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            AStarPath = (ABPath)p;
            if (Vector3.Distance(AStarPath.endPoint, new Vector3(AStarPath.originalEndPoint.x, AStarPath.endPoint.y, AStarPath.originalEndPoint.z)) > 0.5f)
            {
                Debug.Log("���ܵ���Ŀ���");
                AStarPath = null;
            }
            AStarCurrentWaypoint = 1;

        }
        else
        {
            AppDebug.Log("Astar find path fail!");
            AStarPath = null;
        }
    }


    /// <summary>
    /// ���ǰ���A*Ѱ·�Ľ��·���ƶ�
    /// </summary>
    void PlayerAStarMove()
    {
        //-------------------
        //���û��A*·���㣬��ֱ�ӷ���
        //-------------------
        if (AStarPath == null)
        {
            return;
        }

        //�������·����������
        if (AStarCurrentWaypoint >= AStarPath.vectorPath.Count)
        {
            AStarPath = null;//��Ҫ������
            return;
        }

        //===============================
        //��������ת����ƶ�
        //===============================
        Vector3 moveDirection = Vector3.zero;

        Vector3 tempvv = new Vector3(AStarPath.vectorPath[AStarCurrentWaypoint].x, this.transform.position.y, AStarPath.vectorPath[AStarCurrentWaypoint].z);

        //������ת��
        moveDirection = (tempvv - this.transform.position).normalized;//��һ��
        moveDirection.y = 0;
        moveDirection = moveDirection * (Time.deltaTime * 20f);//�ƶ��ٶ�

        if (moveDirection.x != 0 || moveDirection.z != 0)
        {
            //����ת��
            if (moveDirection != Vector3.zero)
            {
                roleSlerp += 3f;
                var rotation = Quaternion.LookRotation(moveDirection);
                this.transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * roleSlerp);

                if (Quaternion.Angle(transform.rotation, rotation) < 1f)
                {
                    roleSlerp = 0f;
                }
            }
        }

        //===============================
        //�ж��Ƿ�Ӧ������һ�����ƶ�
        //===============================
        //Check if we are close enough to the next waypoint
        //If we are, proceed to follow the next waypoint
        float distxx1 = Vector3.Distance(this.gameObject.transform.position, tempvv);
        moveDirection.y = 0;
        if (distxx1 <= moveDirection.magnitude + 0.1f)
        {
            AStarCurrentWaypoint++;
        }


        //�����������ٶȣ��ý�ɫ�������ڵ�����
        moveDirection.y = -100 * Time.deltaTime * 5;

        this.m_CharacterController.Move(moveDirection);//�ƶ���ɫ
    }
}

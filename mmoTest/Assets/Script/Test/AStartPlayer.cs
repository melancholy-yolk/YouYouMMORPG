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
        //玩家点击进行移动
        if (Input.GetMouseButtonDown(0))
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))//判断要移动到的目标点
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
            //开始A*寻路计算，计算完成时将会回调OnPathComplete函数
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
                Debug.Log("不能到达目标点");
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
    /// 主角按照A*寻路的结果路径移动
    /// </summary>
    void PlayerAStarMove()
    {
        //-------------------
        //如果没有A*路径点，则直接返回
        //-------------------
        if (AStarPath == null)
        {
            return;
        }

        //如果整个路径都走完了
        if (AStarCurrentWaypoint >= AStarPath.vectorPath.Count)
        {
            AStarPath = null;//重要！！！
            return;
        }

        //===============================
        //处理主角转身和移动
        //===============================
        Vector3 moveDirection = Vector3.zero;

        Vector3 tempvv = new Vector3(AStarPath.vectorPath[AStarCurrentWaypoint].x, this.transform.position.y, AStarPath.vectorPath[AStarCurrentWaypoint].z);

        //让主角转身
        moveDirection = (tempvv - this.transform.position).normalized;//归一化
        moveDirection.y = 0;
        moveDirection = moveDirection * (Time.deltaTime * 20f);//移动速度

        if (moveDirection.x != 0 || moveDirection.z != 0)
        {
            //缓慢转身
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
        //判断是否应该向下一个点移动
        //===============================
        //Check if we are close enough to the next waypoint
        //If we are, proceed to follow the next waypoint
        float distxx1 = Vector3.Distance(this.gameObject.transform.position, tempvv);
        moveDirection.y = 0;
        if (distxx1 <= moveDirection.magnitude + 0.1f)
        {
            AStarCurrentWaypoint++;
        }


        //考虑重力加速度，让角色紧紧贴在地面上
        moveDirection.y = -100 * Time.deltaTime * 5;

        this.m_CharacterController.Move(moveDirection);//移动角色
    }
}

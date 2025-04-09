using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteeringBehaviour
{

    public int currentWaypoint = 0;

    public Vector2 seek(Vector2 targetPosition, Transform agent, Rigidbody2D agentRb2d, float maxSpeed, float maxForce)
    {
        //obtener la velocidad deseada 
        Vector2 desiredVelocity = targetPosition - (Vector2)agent.position;
        //normalizar la veldeseada para que no salte de golpe
        desiredVelocity.Normalize();
        desiredVelocity *= maxSpeed;
        Vector2 steer = desiredVelocity - agentRb2d.linearVelocity;
        steer = clamp(steer, maxForce);
        steer /= agentRb2d.mass;
        steer = clamp(agentRb2d.linearVelocity + steer, maxSpeed);
        return steer;
    }
    public Vector2 seek(Vector2 targetPosition, Vector2 agent, Rigidbody2D agentRb2d, float maxSpeed, float maxForce)
    {
        //obtener la velocidad deseada 
        Vector2 desiredVelocity = targetPosition - agent;
        //normalizar la veldeseada para que no salte de golpe
        desiredVelocity.Normalize();
        desiredVelocity *= maxSpeed;
        Vector2 steer = desiredVelocity - agentRb2d.linearVelocity;
        steer = clamp(steer, maxForce);
        steer /= agentRb2d.mass;
        steer = clamp(agentRb2d.linearVelocity + steer, maxSpeed);
        return steer;
    }

    public Vector2 flee(Transform target, Transform agent, Rigidbody2D agentRb2d, float maxSpeed, float maxForce)
    {
        return seek(agent.position, target, agentRb2d, maxSpeed, maxForce);
    }

    public Vector2 flee(Vector2 point, Transform agent, Rigidbody2D agentRb2d, float maxSpeed, float maxForce)
    {
        return seek(agent.position, point, agentRb2d, maxSpeed, maxForce);
    }

    public Vector2 arrival(Transform Target, Transform agent, Vector2 velocity, float threshold)
    {
        float distance = Vector2.Distance(Target.position, agent.position);
        if (distance < threshold)
        {
            velocity *= distance / threshold;
        }
        return velocity;
    }

    public Vector2 obstacleAvoidance(Vector2 targetPosition, Transform agent, Vector2 velocity, float maxForce, float prediction)
    {

        Vector2 ahead = (Vector2)agent.position + velocity.normalized * prediction;

        Vector2 Normalvector = new Vector2((targetPosition.x - agent.position.x), (targetPosition.y - agent.position.y));

        Vector2 normalD1 = (new Vector2(-Normalvector.y, Normalvector.x).normalized * maxForce) + targetPosition;
        Vector2 normalD2 = (new Vector2(Normalvector.y, -Normalvector.x).normalized * maxForce) + targetPosition;

        return Vector2.Distance(normalD1, ahead) < Vector2.Distance(normalD2, ahead) ? normalD1 : normalD2;
    }

    public Vector2 wander(Transform agent, Rigidbody2D agentRb2D, float dispValue, float wheel)
    {
        Vector2 displacement = new Vector2(agentRb2D.linearVelocity.x, agentRb2D.linearVelocity.y);
        displacement.Normalize();
        displacement *= dispValue;
        displacement += (Vector2)agent.position;
        Vector2 nextTarget = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
        nextTarget.Normalize();
        nextTarget *= wheel;
        return nextTarget + displacement;
    }

    /*
		<summary>
			Intenta predecir la posición futura del target y seguir esa posición, si está en radio de predictioRadius reduce su tiempo de predicción a futuro para intentar seguir la
			posición actual
		</summary>

		<param name = "agent">Transform del agente </param>
		<param name = "agentRb2d">Rigidbody2d del agente </param>
		<param name = "dispValue"></param>
		<param name = "wheel"> </param>
		<param name = "xMin">Valor mínimo a del que no puede bajar en x </param>
		<param name = "xMax">Valor máximo a del que no puede bajar en x </param>
		<param name = "yMin">Valor mínimo a del que no puede bajar en y </param>
		<param name = "yMax">Valor máximo a del que no puede bajar en y </param>
	*/
    public Vector2 wander(Transform agent, Rigidbody2D agentRb2D, float dispValue, float wheel, float xMin, float xMax, float yMin, float yMax)
    {
        Vector2 displacement = new Vector2(agentRb2D.linearVelocity.x, agentRb2D.linearVelocity.y);
        displacement.Normalize();
        displacement *= dispValue;
        displacement += (Vector2)agent.position;
        Vector2 nextTarget = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
        nextTarget.Normalize();
        nextTarget *= wheel;
        if (agent.position.x >= xMax || agent.position.x <= xMin)
            nextTarget.x *= -1;
        else if (agent.position.y <= -yMin || agent.position.y >= yMax)
            nextTarget.y *= -1;

        return nextTarget + displacement;
    }

    Vector2 clamp(Vector2 Vector, float clampValue)
    {
        if (Vector.magnitude > clampValue)
        {
            Vector.Normalize();
            Vector *= clampValue;
        }
        return Vector;
    }
    /*
		<summary>
			Intenta predecir la posición futura del target y seguir esa posición, si está en radio de predictioRadius reduce su tiempo de predicción a futuro para intentar seguir la
			posición actual
		</summary>

		<param name = "targetPosition"> Vector2 con la posición actual del target del que se tiene que huir</param>
		<param name = "agent">Transform del agente </param>
		<param name = "agentRb2d">Rigidbody2d del agente </param>
		<param name = "targetRb2d">Rigidbody2d del target del que se tiene que huir </param>
		<param name = "maxSpeed">Velocidad máxima del agente </param>
		<param name = "maxForce">Fuerza con la que seguirá al target </param>
		<param name = "predictionRadius">Radio de predicción con el que intenta dejar de predecir para ahora huir solamente </param>
		<param name = "timePredict">Tiempo en el que intenta predecir al futuro en frames </param>
	*/
    public Vector2 pursuit(Vector2 targetPosition, Transform agent, Rigidbody2D agentRb2d, Rigidbody2D targetRb2d, float maxSpeed, float maxForce, float predictionRadius, float timePredict)
    {
        float predictionMultiplier = 1;
        float distance = Vector2.Distance(targetPosition, agent.position);//obtener distancia entre objetivo y agente
        if (distance < predictionRadius)
        {
            predictionMultiplier = distance / predictionRadius;//mejorar la predicci�n entre m�s cerca estemos
        }
        targetPosition += (Vector2)targetRb2d.linearVelocity.normalized * (timePredict * predictionMultiplier);//Adelantarse a la posici�n
        return seek(targetPosition, agent, agentRb2d, maxSpeed, maxForce);
    }

    /*
		<summary>
			Evade al target intentado predecir su posición a futuro
		</summary>

		<param name = "targetPosition"> Vector2 con la posición actual del target del que se tiene que huir</param>
		<param name = "agent">Transform del agente </param>
		<param name = "agentRb2d">Rigidbody2d del agente </param>
		<param name = "targetRb2d">Rigidbody2d del target del que se tiene que huir </param>
		<param name = "maxSpeed">Velocidad máxima del agente </param>
		<param name = "maxForce">Fuerza con la que seguirá al target </param>
		<param name = "predictionRadius">Radio de predicción con el que intenta dejar de predecir para ahora huir solamente </param>
		<param name = "timePredict">Tiempo en el que intenta predecir al futuro en frames </param>
	*/
    public Vector2 evade(Vector2 targetPosition, Transform agent, Rigidbody2D agentRb2d, Rigidbody2D targetRb2d, float maxSpeed, float maxForce, float predictionRadius, float timePredict)
    {
        float predictionMultiplier = 1;
        float distance = Vector2.Distance(targetPosition, agent.position);//obtener distancia entre objetivo y agente
        if (distance < predictionRadius)
        {
            predictionMultiplier = distance / predictionRadius;//mejorar la predicci�n entre m�s cerca estemos
        }
        targetPosition += (Vector2)targetRb2d.linearVelocity.normalized * (timePredict * predictionMultiplier);//Adelantarse a la posici�n
        Vector2 desiredVelocity = (targetPosition - (Vector2)agent.position).normalized * maxSpeed;//se obtiene el vector con la velocidad correcta apuntando al punto deseado
        desiredVelocity *= -1;
        Vector2 currentVelocity = agentRb2d.linearVelocity;
        Vector2 seekForce = (desiredVelocity - currentVelocity).normalized * maxForce;//velocidad de apuntado hacia el objetivo
        Vector2 newVelocity = (currentVelocity + seekForce).normalized * maxSpeed;
        return newVelocity;
    }

    /*
		<summary>
			Se le pasa un lider a seguir y calcula la velocidad, separación para seguirlo
		</summary>

		<param name = "liderPercibed"> Transform del lider </param>
		<param name = "agent">Transform del agente </param>
		<param name = "agentRb2d">Rigidbody2d del agente </param>
		<param name = "maxSpeed">Velocidad máxima del agente </param>
		<param name = "distanceOffSet">Distancia a la que seguirá al lider </param>
		<param name = "seekStrength">Fuerza con la que seguirá y cambiará de ángulo al seguir </param>
		<param name = "arrivalThershold">Radio de proximidad para recalcular velocidad </param>
	*/
    public Vector2 leaderFollow(Transform liderPercibed, Transform agent, Rigidbody2D agentRb2d, float maxSpeed, float distanceOffSet, float seekStrength, float arrivalThershold)
    {
        Vector2 targetPosition = (Vector2)liderPercibed.position - agentRb2d.linearVelocity.normalized * distanceOffSet;//quedarnos atras del lider
        Vector2 desiredVelocity = (targetPosition - (Vector2)agent.position).normalized * maxSpeed;
        Vector2 currentVelocity = agentRb2d.linearVelocity;
        Vector2 steerForce = (desiredVelocity - currentVelocity).normalized * seekStrength;
        Vector2 newVelocity = (currentVelocity + steerForce).normalized * maxSpeed;
        //rb2d.velocity = newVelocity;

        float distance = Vector2.Distance(agent.position, liderPercibed.position);
        if (distance < arrivalThershold)
        {
            //rb2d.velocity *= distance/arrivalThershold; Antes de ser pasado a steering
            return newVelocity *= distance / arrivalThershold;
        }

        return newVelocity;
    }

    /*
		<summary>
			A traves de un array itera para que el agente atraviese cada uno de los puntos en forma secuencial
		</summary>

		<param name = "wayPoints">Un arreglo con los puntos del pathFollowing </param>
		<param name = "agentTransform">Transform del agente</param>
		<param name = "agentRb2d">rigidbody2d del agente</param>
		<param name = "maxSpeed">Velocidad máximca de movimiento</param>
		<param name = "wayPointRange">Rango en el que se toma como verdadero el haber entrado a un punto</param>
		<param name = "seekStrength">Fuerza con la que seguirá un punto</param>
	*/
    public Vector2 PathFollowing(Vector2[] wayPoints, Transform agentTransform, Rigidbody2D agentRb2d, float maxSpeed, float wayPointRange, float seekStrength)
    {
        Vector2 targetPosition = wayPoints[currentWaypoint];
        Vector2 desiredVelocity = (targetPosition - (Vector2)agentTransform.position).normalized * maxSpeed;
        Vector2 currentVelocity = agentRb2d.linearVelocity;
        Vector2 steerForce = (desiredVelocity - currentVelocity).normalized * seekStrength;
        //Cambiar al siguiente waypoint al llegar a uno
        if (Vector2.Distance(targetPosition, (Vector2)agentTransform.position) < wayPointRange)
        {
            ++currentWaypoint;
            if (currentWaypoint >= wayPoints.Length)
            {
                currentWaypoint = 0;
            }
        }
        return (currentVelocity + steerForce).normalized * maxSpeed;
    }

    public Vector2 FollowLider(Transform liderTransform, Transform agentTransform, Rigidbody2D agentRb2d, float seekStrength, float distanceOffSet, float speed, float arrivalThershold)
    {
        Vector2 targetPosition = (Vector2)liderTransform.position - agentRb2d.linearVelocity.normalized * distanceOffSet;
        Vector2 desiredVelocity = (targetPosition - (Vector2)agentTransform.position).normalized * speed;
        Vector2 currentVelocity = agentRb2d.linearVelocity;
        Vector2 steerForce = (desiredVelocity - currentVelocity).normalized * seekStrength;
        Vector2 newVelocity = (currentVelocity + steerForce).normalized * speed;
        float distance = Vector2.Distance(agentTransform.position, liderTransform.position);
        if (distance < arrivalThershold)
        {
            newVelocity *= distance / arrivalThershold;
        }
        return newVelocity;

    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionBehaviour : MonoBehaviour
{
    public enum ActionType
    {
        ATTACK, HEAVY_ATTACK, HEAL
    };

    public ActionType           type;
    public AnimationCurve       attackCurve;
    public float                actionDuration;

    internal CharacterBehaviour user;
    internal int                cooldown = 0;

    private int                 _currentCooldown = 0;

    public IEnumerator Perform(CharacterBehaviour target)
    {
        float counter = 0;
        Vector2 oldPos = user.transform.position,
            targetPos = target.transform.position;
        bool hit = false;

        if (type == ActionType.HEAL)
        {
            Debug.Log(target);
            Debug.Log(_currentCooldown >= cooldown);
        }

        if (target && user &&
            _currentCooldown >= cooldown)
        {
            switch (type)
            {
                case ActionType.ATTACK:
                    do
                    {
                        counter += Time.deltaTime;
                        counter = (counter > actionDuration ? actionDuration : counter);

                        user.transform.position = Vector2.Lerp(oldPos, targetPos, attackCurve.Evaluate(counter / actionDuration));

                        if (!hit &&
                            attackCurve.Evaluate(counter / actionDuration) >= 0.975f)
                        {
                            target.AddHealth(-user.damage);
                            hit = true;
                        }

                        yield return new WaitForSeconds(Time.deltaTime);
                    } while (counter < actionDuration);

                    break;
                case ActionType.HEAVY_ATTACK:
                    do
                    {
                        counter += Time.deltaTime;
                        counter = (counter > actionDuration ? actionDuration : counter);

                        user.transform.position = Vector2.Lerp(oldPos, targetPos, attackCurve.Evaluate(counter / actionDuration));

                        if (!hit &&
                            attackCurve.Evaluate(counter / actionDuration) >= 0.975f)
                        {
                            target.AddHealth(-(user.damage + (user.damage / 2)));
                            hit = true;
                        }

                        yield return new WaitForSeconds(Time.deltaTime);
                    } while (counter < actionDuration);

                    break;
                case ActionType.HEAL:
                    do
                    {
                        counter += Time.deltaTime;
                        counter = (counter > actionDuration ? actionDuration : counter);

                        user.transform.position = Vector2.Lerp(oldPos, targetPos, attackCurve.Evaluate(counter / actionDuration));

                        if (!hit &&
                            attackCurve.Evaluate(counter / actionDuration) >= 0.975f)
                        {
                            target.AddHealth(user.damage);
                            target.GetComponent<PlayerBehaviour>().healParticles.Play();
                            hit = true;
                        }

                        yield return new WaitForSeconds(Time.deltaTime);
                    } while (counter < actionDuration);

                    break;
                default:
                    break;
            }

            _currentCooldown = 0;
        }
        else
        {
            ++_currentCooldown;
        }

        yield return null;
    }

    public void UpdateCooldown()
    {
        ++_currentCooldown;
    }

    public void ReplenishCooldown()
    {
        _currentCooldown = cooldown;
    }
}

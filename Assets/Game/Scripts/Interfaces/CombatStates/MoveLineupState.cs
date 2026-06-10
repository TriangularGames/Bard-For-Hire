using System.Collections.Generic;
using UnityEngine;

public class MoveLineupState : ICombatState
{
    private enum Phase { Wait, Collapse, Done }

    private readonly List<ItemData> _items;
    private readonly int _index;
    private readonly ScoreManager _sm;
    private readonly bool _skipWait;

    private static readonly Vector3 StackOffset = new Vector3(-0.05f, 0f, 0.01f);
    private const float WaitDuration = 0.8f;
    private const float CollapseDuration = 0.35f;

    private Phase _phase;
    private float _timer;

    private ItemManager _im;

    // Sequential collapse: which shift we're currently animating (0 = first move after used item)
    private int _collapseStep;
    private float _lerpTimer;
    private Vector3 _lerpStart;
    private RectTransform _movingRect;
    private bool _transitioned;

    private const float TransitionDelay = 1.0f;
    private float _transitionTimer = 0f;

    public MoveLineupState(List<ItemData> items, int index, ScoreManager sm, bool skipWait = false)
    {
        _items = items;
        _index = index;
        _sm = sm;
        _skipWait = skipWait;
    }

    public void EnterState(CombatManager cm)
    {
        _im = GameObject.FindWithTag("ItemManager").GetComponent<ItemManager>();

        _timer = 0f;
        _phase = Phase.Wait;
        _collapseStep = 0;
        _lerpTimer = 0f;
        _movingRect = null;
        _transitioned = false;
        _transitionTimer = 0f;

        if (_skipWait)
        {
            if (HasRemainingStackItems())
            {
                _phase = Phase.Collapse;
                BeginCollapseStep();
            }
            else
            {
                _phase = Phase.Done;
                _transitionTimer = TransitionDelay;  // no collapse, start transition timer immediately
            }
        }
        else
        {
            _phase = Phase.Wait;
        }
    }

    public void ExitState(CombatManager cm) {}

    public void UpdateState(CombatManager cm)
    {
        if (PauseManager.Instance.IsPaused) return;

        switch (_phase)
        {
            case Phase.Wait:
                UpdateWait(cm);
                break;
            case Phase.Collapse:
                UpdateCollapse(cm);
                break;
            case Phase.Done:
                TransitionNext(cm);
                break;
        }
    }

    private void UpdateWait(CombatManager cm)
    {
        _timer += Time.deltaTime;
        if (_timer < WaitDuration * _sm.GameSpeed) return;

        // Any items left after the one just used?
        if (HasRemainingStackItems())
        {
            _phase = Phase.Collapse;
            BeginCollapseStep();
        }
        else
        {
            _phase = Phase.Done;
        }
    }

    private void UpdateCollapse(CombatManager cm)
    {
        if (_movingRect == null)
        {
            _phase = Phase.Done;
            return;
        }

        _lerpTimer += Time.deltaTime;
        float t = Mathf.Clamp01(_lerpTimer / CollapseDuration);
        float smoothT = Mathf.SmoothStep(0f, 1f, t);

        _movingRect.position = Vector3.Lerp(_lerpStart, GetStackPosition(_collapseStep), smoothT);

        if (t < 1f) return;

        // This shift finished and next item moves forward
        _collapseStep++;
        if (_collapseStep < GetCollapseMoveCount())
        {
            BeginCollapseStep();
        }
        else
        {
            _phase = Phase.Done;
        }
    }

    private void BeginCollapseStep()
    { 
        // Item that was just used is gone; item (_index + 1) moves to slot _index, etc.
        int attackIndex = _index + 1 + _collapseStep;
        GameObject item = _im.GetAttackItem(attackIndex);

        if (item == null)
        {
            _collapseStep++;
            if (_collapseStep < GetCollapseMoveCount())
                BeginCollapseStep();
            else
                _phase = Phase.Done;
            return;
        }

        _movingRect = item.GetComponent<RectTransform>();
        _lerpStart = _movingRect.position;
        _lerpTimer = 0f;

        // Earlier items stay on top
        item.transform.SetSiblingIndex(0);
    }

    private void TransitionNext(CombatManager cm)
    {
        if (_transitioned) return;

        if (_index + 1 < _items.Count)
        {
            _transitioned = true;
            _sm.roller.StartCombatRoll(_items, _index + 1);
            return;  // no TransitionDelay
        }

        if (_transitionTimer < TransitionDelay)
        {
            _transitionTimer += Time.deltaTime * _sm.GameSpeed;
            return;
        }

        _sm.FinalizeRound();
        _transitioned = true;
        cm.SwitchState(new RedrawItemsState(_sm));
    }

    private bool HasRemainingStackItems()
    {
        return _index + 1 < GetAttackItemCount();
    }

    private int GetCollapseMoveCount()
    {
        // After using item _index, items _index+1 .. end-1 each shift forward once
        return Mathf.Max(0, GetAttackItemCount() - _index - 1);
    }

    private int GetAttackItemCount()
    {
        return _im.GetAttackItemCount();
    }

    private Vector3 GetStackPosition(int slotIndex)
    {
        Vector3 displayPos = _sm.itemDisplay.GetComponent<RectTransform>().position;
        return displayPos + StackOffset * slotIndex;
    }
}
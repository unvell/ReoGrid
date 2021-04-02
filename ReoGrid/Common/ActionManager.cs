/*****************************************************************************
 * 
 * ReoGrid - .NET Spreadsheet Control
 * 
 * https://reogrid.net/
 * 
 * ActionManager - Common undo/redo framework for .NET Application
 *  
 * THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
 * KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
 * IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR
 * PURPOSE.
 *
 * Author: Jing Lu <jingwood at unvell.com>
 * 
 * Copyright (c) 2012-2021 Jing Lu <jingwood at unvell.com>
 * Copyright (c) 2012-2016 unvell.com, all rights reserved.
 * 
 ****************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;

namespace unvell.Common
{
	internal sealed class ActionManager
	{
		private static readonly string LOGKEY = "actionmanager";

		public ActionManager()
		{
		}

		private List<IUndoableAction> undoStack = new List<IUndoableAction>();
		private Stack<IUndoableAction> redoStack = new Stack<IUndoableAction>();

		/// <summary>
		/// Get collection of undo action list
		/// </summary>
		public List<IUndoableAction> UndoStack
		{
			get { return undoStack; }
		}

		/// <summary>
		/// Get collection of redo action list
		/// </summary>
		public Stack<IUndoableAction> RedoStack
		{
			get { return redoStack; }
		}

		private int capacity = 30;

		public void DoAction(IAction action)
		{
			DoAction(action, true);
		}

		public void AddAction(IAction action)
		{
			DoAction(action, false);
		}

		private void DoAction(IAction action, bool perform)
		{
			Do(action, perform, (action is IUndoableAction));
		}
		
		/// <summary>
		/// Do specified action.
		/// </summary>
		/// <param name="action">Action to be performed</param>
		/// <param name="perform">True to perform immediately, false to add into stack only</param>
		/// <param name="isCanUndo">Specifies that whether the action can be undone, 
		/// sometimes an action might not necessary to be undone even it implements the 
		/// IUndoable interface.</param>
		private void Do(IAction action, bool perform, bool isCanUndo)
		{
			Logger.Log(LOGKEY, string.Format("{0} action: {1}[{2}]", perform ? "do" : "add", action.GetType().Name, action.GetName()));

			if (BeforePerformAction != null)
			{
				var arg = new ActionEventArgs(action, ActionBehavior.Do);
				BeforePerformAction(this, arg);
				if (arg.Cancel) return;
			}

			if (perform) action.Do();

			if (action is IUndoableAction && isCanUndo)
			{
				redoStack.Clear();
				undoStack.Add(action as IUndoableAction);

				if (undoStack.Count() > capacity)
				{
					int removedActions = undoStack.Count() - capacity;
					undoStack.RemoveRange(0, removedActions);
					Logger.Log(LOGKEY, "action stack full. remove " + (removedActions) + " action(s).");
				}
			}

			AfterPerformAction?.Invoke(this, new ActionEventArgs(action, ActionBehavior.Do));
		}

		/// <summary>
		/// Clear current action stack.
		/// </summary>
		public void Reset()
		{
			redoStack.Clear();
			undoStack.Clear();
		}

		public bool CanUndo()
		{
			return undoStack.Count > 0;
		}

		public bool CanRedo()
		{
			return redoStack.Count() > 0;
		}

		public IAction Undo()
		{
			if (undoStack.Count() > 0)
			{
				IUndoableAction action = null;

				while (undoStack.Count > 0)
				{
					action = undoStack.Last();
					Logger.Log(LOGKEY, "undo action: " + action.ToString());

					// before event
					if (BeforePerformAction != null)
					{
						var arg = new ActionEventArgs(action, ActionBehavior.Undo);
						BeforePerformAction(this, new ActionEventArgs(action, ActionBehavior.Undo));
						if (arg.Cancel) break;
					}

					undoStack.Remove(action);
					action.Undo();
					redoStack.Push(action);

					// after event
					AfterPerformAction?.Invoke(this, new ActionEventArgs(action, ActionBehavior.Undo));

					if (!(action is ISerialUndoAction)) break;
				}

				return action;
			}
			return null;
		}

		public IAction Redo()
		{
			if (redoStack.Count > 0)
			{
				IUndoableAction action = null;

				while (redoStack.Count > 0)
				{
					action = redoStack.Pop();
					Logger.Log(LOGKEY, "redo action: " + action.ToString());

					if (BeforePerformAction != null)
					{
						var arg = new ActionEventArgs(action, ActionBehavior.Redo);
						BeforePerformAction(this, arg);
						if (arg.Cancel) break;
					}

					action.Redo();
					undoStack.Add(action);

					AfterPerformAction?.Invoke(this, new ActionEventArgs(action, ActionBehavior.Redo));

					if (!(action is ISerialUndoAction)) break;
				}

				return action;
			}
			else
				return null;
		}

		public event EventHandler<ActionEventArgs> BeforePerformAction;
		public event EventHandler<ActionEventArgs> AfterPerformAction;
	}

	/// <summary>
	/// Action event argument.
	/// </summary>
	public class ActionEventArgs : EventArgs
	{
		private IAction action;

		/// <summary>
		/// The action is currently performing.
		/// </summary>
		public IAction Action
		{
			get { return action; }
			set { action = value; }
		}

		/// <summary>
		/// The behavior of current action performing. (one of do/undo/redo)
		/// </summary>
		public ActionBehavior Behavior { get; set; }

		/// <summary>
		/// Get or set the Cancel flag to decide whether or not to cancel this operation.
		/// </summary>
		public bool Cancel { get; set; }

		/// <summary>
		/// Construct an argument with specified action and behavior flag.
		/// </summary>
		/// <param name="action">action is currently performing.</param>
		/// <param name="behavior">behavior flag of current operation.</param>
		public ActionEventArgs(IAction action, ActionBehavior behavior)
		{
			this.action = action;
			this.Behavior = behavior;
		}
	}

	/// <summary>
	/// Behavior flag for argument of ActionPerformmed event.
	/// </summary>
	public enum ActionBehavior
	{
		/// <summary>
		/// Do action (action is firstly done)
		/// </summary>
		Do,

		/// <summary>
		/// Redo action (action is redone by ActionManager)
		/// </summary>
		Redo,

		/// <summary>
		/// Undo action (action is undone by ActionManager)
		/// </summary>
		Undo,
	}

	/// <summary>
	/// Represents action interface.
	/// </summary>
	public interface IAction
	{
		/// <summary>
		/// Do this action.
		/// </summary>
		void Do();

		/// <summary>
		/// Get the friendly name of this action.
		/// </summary>
		/// <returns>Get friendly name of action.</returns>
		string GetName();
	}

	/// <summary>
	/// Undoable action interface.
	/// </summary>
	public interface IUndoableAction : IAction
	{
		/// <summary>
		/// Undo this action.
		/// </summary>
		void Undo();

		/// <summary>
		/// Redo this action.
		/// </summary>
		void Redo();
	}

	internal interface ISerialUndoAction : IUndoableAction
	{
	}

	/// <summary>
	/// Action group is used to perform several actions together during one time operation,
	/// For example there is two actions:
	/// <ol>
	/// <li>expend spreadsheet action</li>
	/// <li>copy data action</li>
	/// </ol>
	/// Sometimes it is necessary to perform these two actions together, they are should undo 
	/// together, in this case, create an ActionGroup and add them into the group, then invoke
	/// the 'DoAction' method of 'ActionManager' by passing this action group object.
	/// </summary>
	public class ActionGroup : IUndoableAction
	{
		private List<IAction> actions;

		/// <summary>
		/// Action list stored in this group.
		/// </summary>
		public List<IAction> Actions
		{
			get { return actions; }
			set { actions = value; }
		}

		private string name;

		/// <summary>
		/// Construct action group by specified name, and the collection of action to perform together.
		/// </summary>
		/// <param name="name">Friendly name of this group.</param>
		/// <param name="actions">Collection of action to be performed.</param>
		public ActionGroup(string name, List<IAction> actions)
		{
			this.name = name;
			this.actions = actions;
		}

		/// <summary>
		/// Construct action group by specified name, and the collection of action to perform together.
		/// </summary>
		/// <param name="name">Friendly name of this group.</param>
		public ActionGroup(string name)
		{
			actions = new List<IAction>();
		}

		/// <summary>
		/// Do this action group. (Do all actions that are contained in this group)
		/// </summary>
		public virtual void Do()
		{
			foreach (IAction action in actions)
			{
				action.Do();
			}
		}

		/// <summary>
		/// Undo this action group. (Undo all actions that are contained in this group)
		/// </summary>
		public virtual void Undo()
		{
			for (int i = actions.Count - 1; i >= 0; i--)
				((IUndoableAction)actions[i]).Undo();
		}

		/// <summary>
		/// Redo this action group. (Redo all actions that are contained in this group)
		/// </summary>
		public virtual void Redo()
		{
			this.Do();
		}

		/// <summary>
		/// Get the friendly name of this action group.
		/// </summary>
		/// <returns>Friendly name of this action.</returns>
		public virtual string GetName()
		{
			return name;
		}

		/// <summary>
		/// Convert this action group object into string for displaying.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return string.Format("ActionGroup[" + name + "]");
		}
	}

	/// <summary>
	/// Represents action exception. This exception will be thrown when errors happened during do/undo/redo an action.
	/// </summary>
	[Serializable]
	public class ActionException : Exception
	{
		private IAction action;

		internal IAction Action
		{
			get { return action; }
			set { action = value; }
		}

		/// <summary>
		/// Construct an action exception with specified message.
		/// </summary>
		/// <param name="msg">Message to describe this exception.</param>
		public ActionException(string msg)
			: this(null, msg)
		{
		}

		/// <summary>
		/// Construct an action exception with specified action and message.
		/// </summary>
		/// <param name="action">Action which causes this exception when do/undo/redo.</param>
		/// <param name="msg">Message to describe this exception.</param>
		public ActionException(IAction action, string msg)
			: base(msg)
		{
			this.action = action;
		}
	}
}

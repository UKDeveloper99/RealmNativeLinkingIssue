using System;
using System.Linq;
using System.Collections.Generic;
using Realms;

namespace FaradayAppV2.Core.Services
{
	public interface IRepositoryService : IDisposable
	{
		Realm GetInstance();
		void AddOrUpdate<T>(T item) where T : RealmObject;
		void AddOrUpdate<T>(IEnumerable<T> items) where T : RealmObject;
		void Write(Action writeAction);
		T Query<T>(long id) where T : RealmObject;
		T Query<T>(string id) where T : RealmObject;
		RealmObject Query(string className, long id);
		RealmObject Query(string className, string id);
		IQueryable<dynamic> QueryAll(string className);
		IQueryable<T> QueryAll<T>() where T : RealmObject;
		void RemoveAll();
		void RemoveAll(string className);
		void RemoveAll<T>() where T : RealmObject;
		void Remove<T>(T item) where T : RealmObject;
		void RemoveRange<T>(IQueryable<T> range) where T : RealmObject;
		void Refresh();
		void DeleteRealm();
	}

	public class RepositoryService : IRepositoryService
	{
		private Realm _realm;
		private RealmConfiguration _config;

		public RepositoryService()
		{
			_realm = Realm.GetInstance();
			AttachErrorHandler();
		}

		public RepositoryService(string databasePath)
		{
			_realm = Realm.GetInstance(databasePath);
			AttachErrorHandler();
		}

		public RepositoryService(RealmConfiguration config)
		{
			_config = config;
			_realm = Realm.GetInstance(config);
			AttachErrorHandler();       
		}

		public Realm GetInstance()
		{
			if (_config != null)
				return Realm.GetInstance(_config);
			else
				return Realm.GetInstance();
		}

		private void AttachErrorHandler()
		{
			_realm.Error += OnRealmError;
		}

		public void AddOrUpdate<T>(T item) where T : RealmObject
		{
			_realm.Write(() =>
			{
				_realm.Add<T>(item, true);
			});
		}

		public void AddOrUpdate<T>(IEnumerable<T> items) where T : RealmObject
		{
			if (items.Count() == 0)
				return;

			_realm.Write(() =>
			{
				foreach (var item in items)
				{
					_realm.Add<T>(item, true);
				}
			});
		}

		public void Write(Action writeAction)
		{
			_realm.Write(writeAction);
		}

		public T Query<T>(long id) where T : RealmObject
		{
			return _realm.Find<T>(id);
		}

		public T Query<T>(string id) where T : RealmObject
		{
			if (string.IsNullOrEmpty(id))
				return null;
			return _realm.Find<T>(id);
		}

		public RealmObject Query(string className, long id)
		{
			return _realm.Find(className, id);
		}

		public RealmObject Query(string className, string id)
		{
			if (string.IsNullOrEmpty(id))
				return null;
			return _realm.Find(className, id);
		}

		public IQueryable<dynamic> QueryAll(string className)
		{
			return _realm.All(className);
		}

		public IQueryable<T> QueryAll<T>() where T : RealmObject
		{
			return _realm.All<T>();
		}

		public void RemoveAll()
		{
			_realm.Write(() => { _realm.RemoveAll(); });
		}

		public void RemoveAll(string className)
		{
			_realm.Write(() => { _realm.RemoveAll(className); });
		}

		public void RemoveAll<T>() where T : RealmObject
		{
			_realm.Write(() => { _realm.RemoveAll<T>(); });
		}

		public void Remove<T>(T item) where T : RealmObject
		{
			_realm.Write(() => { _realm.Remove(item); });
		}

		public void RemoveRange<T>(IQueryable<T> range) where T : RealmObject
		{
			_realm.Write(() => { _realm.RemoveRange<T>(range); });
		}

		public void Refresh()
		{
			_realm.Refresh();
		}

		public void Dispose()
		{
			_realm.Dispose();
		}

		public void DeleteRealm()
		{
			Realm.DeleteRealm(_realm.Config);
		}

		private void OnRealmError(object sender, ErrorEventArgs args)
		{
			// TODO Add analytics here
		}

		public void Dispose(bool disposing)
		{
			if (disposing)
			{
				_realm.Error -= OnRealmError;
			}
		}
	}
}

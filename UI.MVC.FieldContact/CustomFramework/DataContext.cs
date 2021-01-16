using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace UI.MVC.FieldContact.CustomFramework
{
    public class DataContext : FCDatabaseEntities
    {
        public DataContext()
        {
            base.Configuration.LazyLoadingEnabled = true;
            base.Configuration.ProxyCreationEnabled = false;
        }

        public T Add<T>(T entity) where T : class
        {
            return base.Set<T>().Add(entity);
        }

        public T Remove<T>(T entity) where T : class
        {
            if (entity is IRowStatus)
            {

            }
            else if (entity is IEntity)
            {
                //LOG
                var removedId = ((IEntity)entity).ID;
                /////
            }
            else
            {

            }

            base.Set<T>().Remove(entity);

            return null;
        }

        public override int SaveChanges()
        {
            base.ChangeTracker.DetectChanges();

            if (base.ChangeTracker.HasChanges())
            {
                //var asd1 = base.ChangeTracker.Entries<IEntity>();
                //var asd2 = base.ChangeTracker.Entries<IRowStatus>();

                foreach (var item in base.ChangeTracker.Entries())
                {
                    switch (item.State)
                    {
                        case EntityState.Added:
                            var currentAdded = item.CurrentValues;
                            if (item.Entity is IRowStatus)
                            {
                                // 1 = New Record
                                (item.Entity as IRowStatus).RowStatusID = 1;
                            }
                            break;
                        case EntityState.Deleted:
                            var originalDeleted = item.OriginalValues;
                            if (item.Entity is IRowStatus)
                            {
                                // 3 = Remove As
                                (item.Entity as IRowStatus).RowStatusID = 3;

                                item.State = EntityState.Modified;
                            }
                            else if (item.Entity is IEntity)
                            {
                                var deletedRowId = (item.Entity as IEntity).ID;
                            }
                            break;
                        case EntityState.Modified:
                            var currentModified = item.CurrentValues;
                            var originalModified = item.OriginalValues;

                            if (item.Entity is IRowStatus)
                            {
                                // 2 = Modified
                                (item.Entity as IRowStatus).RowStatusID = 2;
                            }
                            break;
                        default:
                            break;
                    }
                }
            }

            return base.SaveChanges();
        }
    }
}
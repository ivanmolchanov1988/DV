using DocsVision.Platform.ObjectManager;
using System;
using System.Web;
using DocsVision.Platform.Data.Metadata;
using DocsVision.Platform.ObjectModel.Mapping;
using DocsVision.Platform.ObjectModel;
using DocsVision.Platform.SystemCards.ObjectModel.Mapping;
using DocsVision.BackOffice.ObjectModel.Mapping;
using DocsVision.Platform.SystemCards.ObjectModel.Services;
using DocsVision.Platform.ObjectModel.Persistence;
using DocsVision.BackOffice.ObjectModel.Services;
using DocsVision.BackOffice.ObjectModel;
using System.ComponentModel.Design;

/// <summary>
/// Summary description for CardDataUtils
/// </summary>
public class Context
{
    public Context(HttpApplicationState application)
    {
        this.application = application;
    }

    private HttpApplicationState application;


    private UserSession _userSession = null;
    public UserSession userSession
    {
        get
        {

            if (_userSession == null)
            {
                if (this.application["DocsvsionSession"] != null && SessionIsLive(this.application["DocsvsionSession"]))
                {
                    _userSession = (UserSession)this.application["DocsvsionSession"];
                }
                else
                {
                    SessionManager sessionManager = SessionManager.CreateInstance();

                    sessionManager.Connect(
                        CSXMLConfigManager.GetDVServerURL(),
                        CSXMLConfigManager.GetDVDatabase(),
                        CSXMLConfigManager.GetDVUser(),
                        CSXMLConfigManager.GetDVPassword()
                        );

                    _userSession = sessionManager.CreateSession();

                    this.application.Lock();
                    this.application["DocsvsionSession"] = _userSession;
                    this.application.UnLock();
                }
            }

            return _userSession;
        }
    }

    private bool SessionIsLive(object v)
    {
        if (v == null)
            return false;

        try
        {
            UserSession session = (UserSession)v;
            CardData staffData = session.CardManager.GetDictionaryData(Guid.Empty);
            if (staffData != null)
                return true;

            return false;
        }
        catch
        {
            return false;
        }
    }


    private ObjectContext _objectContext = null;
    public ObjectContext objectContext
    {
        get
        {
            if (_objectContext == null)
            {
                _objectContext = createObjectContext(this.userSession);
            }

            return _objectContext;
        }
    }




    private ObjectContext createObjectContext(UserSession userSession)
    {
        var sessionContainer = new ServiceContainer();
        sessionContainer.AddService(typeof(UserSession), userSession);

        var objectContext = new ObjectContext(sessionContainer);

        var mapperFactoryRegistry = objectContext.GetService<IObjectMapperFactoryRegistry>();
        mapperFactoryRegistry.RegisterFactory(typeof(SystemCardsMapperFactory));
        mapperFactoryRegistry.RegisterFactory(typeof(BackOfficeMapperFactory));


        var serviceFactoryRegistry = objectContext.GetService<IServiceFactoryRegistry>();
        serviceFactoryRegistry.RegisterFactory(typeof(BackOfficeServiceFactory));
        serviceFactoryRegistry.RegisterFactory(typeof(SystemCardsServiceFactory));


        ILogService logService = objectContext.GetService<ILogService>();
        mapperFactoryRegistry.RegisterFactory(typeof(ILogService));

        objectContext.AddService<IPersistentStore>(DocsVisionObjectFactory.CreatePersistentStore(new SessionProvider(userSession), null));

        IMetadataProvider metadataProvider = DocsVisionObjectFactory.CreateMetadataProvider(userSession);
        objectContext.AddService<IMetadataManager>(DocsVisionObjectFactory.CreateMetadataManager(metadataProvider, userSession));
        objectContext.AddService<IMetadataProvider>(metadataProvider);

        return objectContext;
    }
}
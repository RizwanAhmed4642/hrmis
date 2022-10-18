using Hrmis.Models.DbModel;
using Hrmis.Models.Dto;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;

namespace Hrmis.Models.Services
{
    public class MWRouteService
    {
        public AppResponse<List<City>> GetCities()
        {
            using (var _db = new HR_System())
            {
                try
                {
                    var cities = _db.Cities.Where(x => x.IsActive == true).OrderBy(k => k.Name).ToList();
                    return new AppResponse<List<City>>() { Data = cities, IsException = false, Messages = "Success" };
                }
                catch (Exception ex1)
                {
                    throw;
                }
            }
        }
        public AppResponse<List<MWPointType>> GetPointTypes()
        {
            using (var _db = new HR_System())
            {
                try
                {
                    var pointTypes = _db.MWPointTypes.ToList();
                    return new AppResponse<List<MWPointType>>() { Data = pointTypes, IsException = false, Messages = "Success" };
                }
                catch (Exception ex1)
                {
                    throw;
                }
            }
        }
        public AppResponse<List<MWRouteDetailView>> GetRouteDetails(int SourceId, int DestinationId)
        {
            using (var _db = new HR_System())
            {
                try
                {
                    var routesDetails = _db.MWRouteDetailViews.Where(x => x.SourceId == SourceId && x.DestinationId == DestinationId && x.IsActive == true).OrderByDescending(x => x.DistanceFromDestination).ToList();
                    return new AppResponse<List<MWRouteDetailView>>() { Data = routesDetails, IsException = false, Messages = "Success" };
                }
                catch (Exception ex1)
                {
                    throw;
                }
            }
        }
        public AppResponse<MWRoute> SaveRoute(MWRoute mwRoute, string userName, string userId)
        {
            using (var _db = new HR_System())
            {
                try
                {
                    if (mwRoute.Id == 0 && mwRoute.DestinationId > 0 && mwRoute.SourceId > 0)
                    {
                        mwRoute.UserId = userId;
                        mwRoute.CreatedBy = userName;
                        mwRoute.CreatedDate = DateTime.UtcNow.AddHours(5);
                        mwRoute.IsActive = true;
                        _db.MWRoutes.Add(mwRoute);
                        _db.SaveChanges();
                        return new AppResponse<MWRoute>() { Data = mwRoute, IsException = false, Messages = "Success" };
                    }
                }
                catch (Exception ex1)
                {
                    throw;
                }
            }
            return null;
        }
        public bool RemoveRoute(int Id)
        {
            using (var _db = new HR_System())
            {
                var mwRoute = _db.MWRoutes.FirstOrDefault(x => x.Id == Id);
                try
                {
                    if (mwRoute != null)
                    {
                        mwRoute.IsActive = false;
                        _db.SaveChanges();
                        return true;
                    }
                    return false;
                }
                catch (Exception ex1)
                {
                    throw;
                }
            }
        }
        public AppResponse<MWRouteDetail> SaveRouteDetail(MWRouteDetail mwRouteDetail, string userName, string userId)
        {
            using (var _db = new HR_System())
            {
                try
                {
                    if (
                        mwRouteDetail.Id == 0 && mwRouteDetail.PointTypeId > 0
                        && mwRouteDetail.RouteId > 0
                        && mwRouteDetail.Latitude > 0
                        && mwRouteDetail.Longitude > 0
                        )
                    {
                        mwRouteDetail.UserId = userId;
                        mwRouteDetail.CreatedBy = userName;
                        mwRouteDetail.CreatedDate = DateTime.UtcNow.AddHours(5);
                        mwRouteDetail.IsActive = true;
                        _db.MWRouteDetails.Add(mwRouteDetail);
                        _db.SaveChanges();
                        return new AppResponse<MWRouteDetail>() { Data = mwRouteDetail, IsException = false, Messages = "Success" };
                    }
                }
                catch (Exception ex1)
                {
                    throw;
                }
            }
            return null;
        }
        public AppResponse<MWRouteDetail> SaveRoutes(MWRouteDto mwRouteDto, string userName, string userId)
        {
            using (var _db = new HR_System())
            {
                try
                {
                    if (mwRouteDto.PointTypeId > 0
                        && mwRouteDto.SourceId > 0
                        && mwRouteDto.DestinationId > 0
                        && mwRouteDto.Latitude > 0
                        && mwRouteDto.Longitude > 0)
                    {
                        var route = _db.MWRoutes.FirstOrDefault(x => x.SourceId == mwRouteDto.SourceId && x.DestinationId == mwRouteDto.DestinationId && x.IsActive == true);
                        MWRouteDetail mwRouteDetail = new MWRouteDetail();
                        if (route == null)
                        {
                            MWRoute mwRoute = new MWRoute();
                            mwRoute.SourceId = mwRouteDto.SourceId;
                            mwRoute.DestinationId = mwRouteDto.DestinationId;
                            mwRoute.SourceId = mwRouteDto.SourceId;
                            var res = SaveRoute(mwRoute, userName, userId);
                            if (res != null)
                            {
                                mwRoute = res.Data;
                                mwRouteDetail.RouteId = mwRoute.Id;
                                mwRouteDetail.PointTypeId = mwRouteDto.PointTypeId;
                                mwRouteDetail.Latitude = mwRouteDto.Latitude;
                                mwRouteDetail.Longitude = mwRouteDto.Longitude;
                            }
                        }
                        else
                        {
                            mwRouteDetail.RouteId = route.Id;
                            mwRouteDetail.PointTypeId = mwRouteDto.PointTypeId;
                            mwRouteDetail.Latitude = mwRouteDto.Latitude;
                            mwRouteDetail.Longitude = mwRouteDto.Longitude;
                        }
                        mwRouteDetail.UserId = userId;
                        mwRouteDetail.CreatedBy = userName;
                        mwRouteDetail.CreatedDate = DateTime.UtcNow.AddHours(5);
                        mwRouteDetail.IsActive = true;
                        var distanceData = DistanceSourceDestination(mwRouteDto);
                        if(distanceData != null)
                        {
                            mwRouteDetail.DistanceFromDestination = distanceData.DistanceFromDestination;
                            mwRouteDetail.DistanceFromDestinationText = distanceData.DistanceFromDestinationText;
                        }
                        _db.MWRouteDetails.Add(mwRouteDetail);
                        _db.SaveChanges();
                        return new AppResponse<MWRouteDetail>() { Data = mwRouteDetail, IsException = false, Messages = "Success" };
                    }
                }
                catch (Exception ex1)
                {
                    throw;
                }
            }
            return null;
        }
        public bool RemoveRouteDetail(int Id)
        {
            using (var _db = new HR_System())
            {
                var mwRouteDetail = _db.MWRouteDetails.FirstOrDefault(x => x.Id == Id);
                try
                {
                    if (mwRouteDetail != null)
                    {
                        mwRouteDetail.IsActive = false;
                        _db.SaveChanges();
                        return true;
                    }
                    return false;
                }
                catch (Exception ex1)
                {
                    throw;
                }
            }
        }
        public AppResponse<List<TagLocationCount>> GetTagLocationsCount()
        {
            using (var _db = new HR_System())
            {
                try
                {
                    var tagLocations = _db.MWRouteDetailViews.GroupBy(x => new { x.PointTypeName }).Select(x => new TagLocationCount
                    {
                        Name = x.Key.PointTypeName,
                        Count = x.Count()
                    }).ToList();
                    return new AppResponse<List<TagLocationCount>>() { Data = tagLocations, IsException = false, Messages = "Success" };
                }
                catch (Exception ex1)
                {
                    throw;
                }
            }
        }
        public AppResponse<List<TagLocationCount>> GetRoutes()
        {
            using (var _db = new HR_System())
            {
                try
                {
                    var routes = _db.MWRouteViews.GroupBy(x => new
                    {
                        x.SourceId,
                        x.DestinationId,
                        x.SourceName,
                        x.DestinationName,
                        x.SourceLatitude,
                        x.SourceLongitude,
                        x.DestinationLatitude,
                        x.DestinationLongitude
                    }).Select(x => new TagLocationCount
                    {
                        Name = x.Key.SourceName + " - " + x.Key.DestinationName,
                        SourceId = x.Key.SourceId,
                        DestinationId = x.Key.DestinationId,
                        SourceLatitude = x.Key.SourceLatitude,
                        SourceLongitude = x.Key.SourceLongitude,
                        DestinationLatitude = x.Key.DestinationLatitude,
                        DestinationLongitude = x.Key.DestinationLongitude,
                        Count = x.Count()
                    }).ToList();
                    return new AppResponse<List<TagLocationCount>>() { Data = routes, IsException = false, Messages = "Success" };
                }
                catch (Exception ex1)
                {
                    throw;
                }
            }
        }

        public MWRouteDetail DistanceSourceDestination(MWRouteDto mwRouteDto)
        {
            try
            {
                using (var _db = new HR_System())
                {
                    var destination = _db.Cities.FirstOrDefault(x => x.Id == mwRouteDto.DestinationId);
                    if (destination == null) return null;
                    string origin = mwRouteDto.Latitude + "," + mwRouteDto.Longitude;
                    string destinations = destination.Latitude + "%2C" + destination.Longitude;
                    MWRouteDetail mwRouteDetail = new MWRouteDetail();
                    if (!string.IsNullOrEmpty(destinations))
                    {
                        RootObject result = new RootObject();
                        string url = @"https://maps.googleapis.com/maps/api/distancematrix/json?units=metric&alternatives=true&origins=" + origin + "&destinations=" + destinations + "&key=AIzaSyDhK39-ZBuptKm4OabOMehNNRYL7DXalgI";

                        WebRequest request = WebRequest.Create(url);
                        request.Credentials = CredentialCache.DefaultCredentials;

                        WebResponse response = request.GetResponse();
                        using (Stream dataStream = response.GetResponseStream())
                        {
                            StreamReader reader = new StreamReader(dataStream);
                            string responseFromServer = reader.ReadToEnd();
                            result = JsonConvert.DeserializeObject<RootObject>(responseFromServer);

                            if (result.rows[0].elements.Count > 0 && result.rows[0].elements[0].distance != null)
                            {
                                mwRouteDetail.DistanceFromDestinationText = result.rows[0].elements[0].distance.text;
                                mwRouteDetail.DistanceFromDestination = result.rows[0].elements[0].distance.value;
                            }

                        }
                        response.Close();
                        return mwRouteDetail;
                    }
                    return null;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public class TagLocationCount
        {
            public string Name { get; set; }
            public int Count { get; set; }
            public int? SourceId { get; set; }
            public Nullable<double> SourceLatitude { get; set; }
            public Nullable<double> SourceLongitude { get; set; }
            public int? DestinationId { get; set; }
            public Nullable<double> DestinationLatitude { get; set; }
            public Nullable<double> DestinationLongitude { get; set; }

        }
        public partial class MWRouteDto
        {
            public Nullable<int> PointTypeId { get; set; }
            public Nullable<int> SourceId { get; set; }
            public Nullable<int> DestinationId { get; set; }
            public Nullable<double> Latitude { get; set; }
            public Nullable<double> Longitude { get; set; }
        }
    }
}
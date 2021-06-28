package be.kdg.stemtest.model.rest

import be.kdg.stemtest.model.entity.*
import io.reactivex.Completable
import io.reactivex.Single
import retrofit2.http.*

interface StemTestService {


    @POST("session/AddStudent/{sessionCode}")
    fun addStudent(@Path("sessionCode") sessioncode:Int) : Single<Int>

    @PUT("session/SelectParty/{sessionId}/{studentId}/{selectedParty}")
    fun selectParty(@Path("sessionId") sessionCode: Int, @Path("studentId") studentId: Int, @Path("selectedParty") party:String) : Completable

    @GET("session/GetStatement/{sessionCode}/{statementIndex}")
    fun getStatementByIndex(@Path("sessionCode") sessionCode: Int, @Path("statementIndex") statementIndex: Int) : Single<Statement>

    @GET("session/GetAnswerOptions/{sessionCode}/{statementIndex}")
    fun getAnswerOptionByIndex(@Path("sessionCode") sessionCode: Int, @Path("statementIndex") statementIndex: Int) : Single<List<AnswerOption>>

    @PUT("answer/AnswerStatement/{sessionCode}/{studentId}/{argument}/{answerOptionId}")
    fun answerStatement(@Path("sessionCode") sessionCode: Int, @Path("studentId") studentId :Int, @Path("argument") argument:String?, @Path("answerOptionId") answerOption:Int) : Completable

    @GET("session/EndGame/{sessionId}/{studentId}")
    fun endPartyGame(@Path("sessionId") sessionCode: Int, @Path("studentId") studentId:Int) : Single<String>

    @GET("answer/GetPartyAnswers/{sessionCode}/{partyName}")
    fun getPartyAnswers(@Path("sessionCode") sessionCode: Int, @Path("partyName") partyName:String) : Single<List<PartyAnswer>>

    @GET("session/GetChosenParties/{sessionCode}")
    fun getParties(@Path("sessionCode") sessionCode: Int): Single<List<Party>>

    @GET("session/GetGameSettings/{sessionCode}")
    fun getSettings(@Path ("sessionCode") sessionCode: Int) : Single<GameSettings>

    @GET("session/EndDebateGame/{sessionCode}/{studentId}")
    fun endDebateGame(@Path("sessionCode") sessionCode: Int, @Path("studentId") studentId: Int): Single<List<DebateGameResult>>

    @GET("session/EndCustomPartyGame/{sessionId}/{studentId}")
    fun endCustomPartyGame(@Path("sessionId")sessionCode: Int, @Path("studentId") studentCode: Int): Single<String>

    @GET("answer/GetCPgAnswers/{sessionCode}")
    fun getCorrectAnswers(@Path("sessionCode")sessionCode: Int): Single<List<AnswerOption>>

    @GET("session/GetDefinitions/{sessionCode}")
    fun getDefinitions(@Path("sessionCode") sesionCode:Int): Single<List<Definition>>

    @GET("answer/GetAllPartyAnswers/{sessionCode}")
    fun getAllPartyAnswers(@Path("sessionCode")sessionCode: Int): Single<List<PartyAnswer>>

    @GET("session/GetCustomDebateGameResults/{sessionCode}")
    fun getCustomDebateGameResult(@Path("sessionCode")sessionCode: Int): Single<List<CustomDebateGameResult> >
}
package be.kdg.stemtest.model.Interactors

import be.kdg.stemtest.model.entity.AnswerOption
import be.kdg.stemtest.model.entity.CustomDebateGameResult
import be.kdg.stemtest.model.entity.DebateGameResult
import be.kdg.stemtest.model.entity.PartyAnswer
import be.kdg.stemtest.model.rest.RemoteDataSource
import io.reactivex.Single
import io.reactivex.schedulers.Schedulers
import javax.inject.Inject

class ResultRemoteInteractor @Inject constructor(private val databaseInteractor: ResultDatabaseInteractor,
                                                 private val remoteDataSource: RemoteDataSource,
                                                 private val identificationDatabaseInteractor: ConnectionDatabaseInteractor,
                                                 private val partyDatabaseInteractor: PartyDatabaseInteractor) {

    fun getPartyGameResult(): Single<String> {

        val identification = identificationDatabaseInteractor.getData()
            .subscribeOn(Schedulers.io())
            .observeOn(Schedulers.io())
            .blockingGet()

        return remoteDataSource.getStemTestService().endPartyGame(identification.sessionCode,identification.studentId)
            .doOnSuccess { i -> databaseInteractor.savePartyResult(i) }
    }


    fun getPartyAnswers(): Single<List<PartyAnswer>> {
        val chosenParty = partyDatabaseInteractor
            .getChosenParty()
            .subscribeOn(Schedulers.io())
            .observeOn(Schedulers.io())
            .blockingGet()
            .name

        val sessionCode = identificationDatabaseInteractor
            .getData()
            .subscribeOn(Schedulers.io())
            .observeOn(Schedulers.io())
            .blockingGet()
            .sessionCode


        return remoteDataSource.getStemTestService().getPartyAnswers(sessionCode, chosenParty)
            .doOnSuccess { i -> databaseInteractor.saveAntwoord(i) }

    }

    fun getDebateResult(): Single<List<DebateGameResult>> {
        val identification = identificationDatabaseInteractor.getData()
            .subscribeOn(Schedulers.io())
            .observeOn(Schedulers.io())
            .blockingGet()

        return remoteDataSource.getStemTestService().endDebateGame(identification.sessionCode,identification.studentId)
            .doOnSuccess { i -> databaseInteractor.saveDebateResult(i) }
    }

    fun getCustomPartyGameResult(): Single<String> {
        val identification = identificationDatabaseInteractor.getData()
            .subscribeOn(Schedulers.io())
            .observeOn(Schedulers.io())
            .blockingGet()

        return remoteDataSource.getStemTestService().endCustomPartyGame(identification.sessionCode,identification.studentId)
            .doOnSuccess { i -> databaseInteractor.savePartyResult(i) }
    }

    fun getCorrectAnswers(): Single<List<AnswerOption>> {

        val sessionCode = identificationDatabaseInteractor
            .getData()
            .subscribeOn(Schedulers.io())
            .observeOn(Schedulers.io())
            .blockingGet()
            .sessionCode


        return remoteDataSource.getStemTestService().getCorrectAnswers(sessionCode)
            .doOnSuccess { i -> databaseInteractor.saveCPGameCorrectAnswers(i) }
    }

    fun getAllPartyAnswers(): Single<List<PartyAnswer>> {
        val sessionCode = identificationDatabaseInteractor
            .getData()
            .subscribeOn(Schedulers.io())
            .observeOn(Schedulers.io())
            .blockingGet()
            .sessionCode

        return remoteDataSource.getStemTestService().getAllPartyAnswers(sessionCode)
    }

    fun getCustomDebateGameResults(index : Int ): Single<CustomDebateGameResult> {
        val sessionCode = identificationDatabaseInteractor
            .getData()
            .subscribeOn(Schedulers.io())
            .observeOn(Schedulers.io())
            .blockingGet()
            .sessionCode

        return remoteDataSource.getStemTestService().getCustomDebateGameResult(sessionCode)
            .doOnSuccess { i -> databaseInteractor.saveCDgResults(i) }
            .map { i -> i.find { j -> j.id == index  } }
    }

}
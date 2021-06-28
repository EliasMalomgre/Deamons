package be.kdg.stemtest.model.Interactors

import be.kdg.stemtest.model.entity.Statement
import be.kdg.stemtest.model.rest.RemoteDataSource
import io.reactivex.Completable
import io.reactivex.Single
import io.reactivex.android.schedulers.AndroidSchedulers
import io.reactivex.schedulers.Schedulers
import javax.inject.Inject

class StatementRemoteInteractor @Inject constructor(private val databaseInteractor: StatementDatabaseInteractor,
                                                    private val identificationDatabaseInteractor: ConnectionDatabaseInteractor,
                                                    private val remoteDataSource: RemoteDataSource) {

    fun getStatement(index: Int): Single<Statement> {

        val sessionCode = identificationDatabaseInteractor.getData()
            .subscribeOn(Schedulers.io())
            .observeOn(Schedulers.io())
            .blockingGet()
            .sessionCode

        val answerOptions = remoteDataSource.getStemTestService().getAnswerOptionByIndex(sessionCode,index)
            .subscribeOn(Schedulers.io())
            .observeOn(Schedulers.io())
            .doOnSuccess{ i -> databaseInteractor.saveAnswerOptions(i,index+1) }

        val statements = remoteDataSource.getStemTestService().getStatementByIndex(sessionCode,index)
            .doOnSuccess(databaseInteractor::saveStatement)
            .doOnSuccess { answerOptions
                            .subscribe() }
        return statements
            .subscribeOn(Schedulers.io())
            .observeOn(AndroidSchedulers.mainThread())
    }


    fun pushAnswer(argument:String?, answerOptionId:Int) : Completable{
        val identification = identificationDatabaseInteractor.getData()
            .subscribeOn(Schedulers.io())
            .observeOn(Schedulers.io())
            .blockingGet()

       return remoteDataSource.getStemTestService().answerStatement(identification.sessionCode,identification.studentId,argument,answerOptionId)
    }

}